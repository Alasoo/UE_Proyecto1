using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using MyExtensions;
using System;
using EnemySystem;


namespace GameSystem
{
    public class EnemyCreator : Singleton<EnemyCreator>
    {
        [Header("MAP CREATOR")]
        [SerializeField] private MapCreator mapCreator;

        [Header("ENEMIES")]
        [SerializeField] private List<WaveScriptable> waves = new();

        private CancellationTokenSource ctsCreator;

        private Dictionary<EnemyController, Vector3> enemyList = new();

        private void OnDestroy()
        {
            ctsCreator?.ClearCts();
            ctsCreator = null;
        }


        [ContextMenu("Init")]
        public async UniTask Init()
        {
            ClearMap();

            ctsCreator?.Cancel();
            ctsCreator = new();
            try
            {
                WaveScriptable wave = waves.RandomElement();
                foreach (WaveData waveData in wave.waveData)
                {
                    int amountToSpawn = UnityEngine.Random.Range(waveData.minEnemies, waveData.maxEnemies + 1);

                    int maxAttempts = 20;

                    for (int i = 0; i < amountToSpawn; i++)
                    {
                        ctsCreator.Token.ThrowIfCancellationRequested();
                        EnemyController newEnemy = Instantiate(waveData.enemyScriptable.enemyControllerPrefab, transform);
                        newEnemy.spriteRenderer.sprite = waveData.enemyScriptable.sprite;
                        newEnemy.transform.name = $"Enemy{wave.waveData.IndexOf(waveData)}_{i}/{amountToSpawn-1}";
                        float enemyRadius = newEnemy.spriteRenderer != null ? newEnemy.spriteRenderer.sprite.bounds.extents.x : 0.5f;
                        int cellRadius = Mathf.CeilToInt(enemyRadius);

                        bool placed = false;
                        int attempts = 0;

                        while (!placed && attempts < maxAttempts)
                        {
                            ctsCreator.Token.ThrowIfCancellationRequested();

                            Vector3Int randomCell = new Vector3Int(
                                UnityEngine.Random.Range(mapCreator.margin, mapCreator.currentBiome.size.x - mapCreator.margin),
                                UnityEngine.Random.Range(mapCreator.margin, mapCreator.currentBiome.size.y - mapCreator.margin),
                                0
                            );
                            Vector3 potentialWorldPos = mapCreator.groundTilemap.GetCellCenterWorld(randomCell);

                            if (await mapCreator.IsAreaFree(randomCell, cellRadius, potentialWorldPos))
                            {
                                Vector3 worldPos = mapCreator.groundTilemap.GetCellCenterWorld(randomCell);
                                newEnemy.transform.position = worldPos;
                                placed = true;
                                enemyList.Add(newEnemy, worldPos);
                            }

                            attempts++;
                        }

                        if (!placed)
                        {
                            Debug.LogWarning($"Could not place enemy after {maxAttempts} attempts.");
                            Destroy(newEnemy.gameObject);
                        }

                        mapCreator.ReportLocalProgress((float)(i + 1) / amountToSpawn * wave.waveData.Count);
                        await UniTask.Yield(cancellationToken: ctsCreator.Token);
                    }


                    /*
                    for (int i = 0; i < amountToSpawn; i++)
                    {
                        Vector3 spawnPosition = transform.position; // checkear con el mapCreator si tengo hueco

                        EnemyController newEnemy = Instantiate(
                            waveData.enemyScriptable.enemyControllerPrefab,
                            spawnPosition,
                            Quaternion.identity
                        );

                        newEnemy.Init(waveData.enemyScriptable);

                        await UniTask.Yield(ctsCreator.Token);
                    }
                    */
                }

            }
            catch (OperationCanceledException)
            {
                Debug.Log($"Canceled");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
            }
            finally
            {
                ctsCreator?.ClearCts();
                ctsCreator = null;
            }
        }




        private void ClearMap()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            enemyList.Clear();

        }

    }
}
