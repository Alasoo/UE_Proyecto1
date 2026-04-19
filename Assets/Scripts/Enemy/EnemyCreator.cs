using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using MyExtensions;
using System;
using EnemySystem;
using Controller.Player;
using Controller.Enemy;
using BulletSystem;


namespace GameSystem
{
    public class EnemyCreator : Singleton<EnemyCreator>
    {
        [Header("MAP CREATOR")]
        [SerializeField] private MapCreator mapCreator;
        [Header("PLAYER")]
        [SerializeField] private PlayerStateMachine playerStateMachine;
        [SerializeField] private float safeDistanceToEnemies = 15f; // Distancia minima a los enemigos

        [Header("ENEMIES")]
        [SerializeField] private List<WaveScriptable> waves = new();
        [Header("POOL CREATOR")]
        [SerializeField] private BulletPool bulletPool;


        private CancellationTokenSource ctsCreator;

        private Dictionary<EnemyStateMachine, Vector3> enemyList = new();

        private void OnDestroy()
        {
            ctsCreator?.ClearCts();
            ctsCreator = null;
        }


        public async UniTask Init()
        {
            ctsCreator?.Cancel();
            ctsCreator = new();
            try
            {
                await CreateNPCs();
                await FindPlayerPosition();
                playerStateMachine.SetCanMove(true);
                Debug.Log($"Fin");
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



        private async UniTask CreateNPCs()
        {
            WaveScriptable wave = waves.RandomElement();
            foreach (WaveData waveData in wave.waveData)
            {
                int amountToSpawn = UnityEngine.Random.Range(waveData.minEnemies, waveData.maxEnemies + 1);

                int maxAttempts = 20;

                for (int i = 0; i < amountToSpawn; i++)
                {
                    ctsCreator.Token.ThrowIfCancellationRequested();
                    EnemyStateMachine newEnemy = Instantiate(waveData.enemyScriptable.TakeCharacterPrefab(), transform);
                    newEnemy.spriteRenderer.sprite = waveData.enemyScriptable.sprite;
                    newEnemy.transform.name = $"Enemy{wave.waveData.IndexOf(waveData)}_{i}/{amountToSpawn - 1}";
                    newEnemy.transform.eulerAngles = new Vector3(-90f, 0, 0);
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
                            newEnemy.Init(waveData.enemyScriptable);
                            var bulletScriptable = waveData.enemyScriptable.TakeBulletScriptable();
                            if (bulletScriptable != null)
                            {
                                var bulletData = bulletScriptable.TakeBulletPrefab();
                                if (bulletData.bullet != null && bulletData.count > 0)
                                    await bulletPool.CreateBullet(bulletData.bullet, bulletData.count, bulletScriptable);
                            }
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
            }
        }


        private async UniTask FindPlayerPosition()
        {
            int maxAttempts = 50; // Le damos varios intentos para encontrar el sitio perfecto
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

                // 1. Comprobamos que no haya agua, montañas o árboles (usamos un radio pequeño como 1 o 2)
                bool environmentFree = await mapCreator.IsAreaFree(randomCell, 1, potentialWorldPos);

                if (environmentFree)
                {
                    // 2. Comprobamos la distancia respecto a todos los enemigos creados
                    bool isSafeFromEnemies = true;
                    foreach (Vector3 enemyPos in enemyList.Values)
                    {
                        if (Vector3.Distance(potentialWorldPos, enemyPos) < safeDistanceToEnemies)
                        {
                            isSafeFromEnemies = false;
                            break; // Está demasiado cerca de un enemigo, cancelamos esta posición
                        }
                    }

                    if (isSafeFromEnemies)
                    {
                        playerStateMachine.transform.position = potentialWorldPos;
                        placed = true;
                        return; // Posición encontrada con éxito
                    }
                }

                attempts++;
                await UniTask.Yield(cancellationToken: ctsCreator.Token);
            }

            Debug.LogWarning("No se pudo encontrar una posición segura para el jugador. Considera reducir la cantidad de enemigos o el radio de seguridad.");
        }


        public void ClearNpcs()
        {
            bulletPool.ClearBullets();

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            enemyList.Clear();
        }

        private void OnDrawGizmos()
        {
            // Dibujamos el radio de seguridad alrededor del jugador para debug
            if (playerStateMachine != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(playerStateMachine.transform.position, safeDistanceToEnemies);
            }
        }

    }
}
