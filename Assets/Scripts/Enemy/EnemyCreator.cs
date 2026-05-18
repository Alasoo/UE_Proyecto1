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
using UnityEngine.AI;


namespace GameSystem
{
    public class EnemyCreator : Singleton<EnemyCreator>
    {
        [Header("MAP CREATOR")]
        [SerializeField] private MapCreator mapCreator;
        [Header("PLAYER")]
        [SerializeField] private PlayerStateMachine playerStateMachine;
        //[SerializeField] private float safeDistanceToEnemies = 15f; // Distancia minima a los enemigos

        [Header("ENEMIES")]
        [SerializeField] private List<WaveScriptable> waveScriptables = new();
        [Header("POOL CREATOR")]
        [SerializeField] private BulletPool bulletPool;

        private CancellationTokenSource ctsCreator;
        private List<CancellationTokenSource> ctsFindPos = new();
        private int totalRangedEnemies = 0;
        private int totalMeleeEnemies = 0;

        private Dictionary<WaveData, List<EnemyStateMachine>> waves = new();


        private void OnDestroy()
        {
            Extensions.ClearCts(ref ctsCreator);

            for (int i = 0; i < ctsFindPos.Count; i++)
            {
                ctsFindPos[i]?.ClearCts();
            }
            ctsFindPos.Clear();

            if (GameManager.Instance != null)
                GameManager.Instance.OnUpdateTime -= OnUpdateTime;
        }

        void Start()
        {
            GameManager.Instance.OnUpdateTime += OnUpdateTime;
        }


        public async UniTask Init()
        {
            ctsCreator?.Cancel();
            ctsCreator = new();
            try
            {
                await FindPlayerPosition();
                await CreateNPCs();
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


        private void OnUpdateTime(int second)
        {
            foreach (var wave in waves)
            {
                if (wave.Key.secondToSpawn <= second && !wave.Key.spawned)
                {
                    wave.Key.spawned = true;
                    Debug.Log($"Tengo que encender: {wave.Value.Count} enemigos");
                    _ = SpawnWaveAsync(wave.Key, wave.Value);
                }
            }
        }



        private async UniTask CreateNPCs()
        {
            WaveScriptable wave = waveScriptables.RandomElement();

            int totalWaves = wave.waveData.Count;
            int currentWaveIndex = 0;

            foreach (WaveData waveData in wave.waveData)
            {
                int amountToSpawn = UnityEngine.Random.Range(waveData.minEnemies, waveData.maxEnemies + 1);

                for (int i = 0; i < amountToSpawn; i++)
                {
                    ctsCreator.Token.ThrowIfCancellationRequested();
                    EnemyStateMachine newEnemy = Instantiate(waveData.enemyScriptable.TakeCharacterPrefab(), Vector3.zero, Quaternion.identity, transform);
                    newEnemy.spriteRenderer.sprite = waveData.enemyScriptable.sprite;
                    newEnemy.transform.name = $"Enemy{wave.waveData.IndexOf(waveData)}_{i}/{amountToSpawn - 1}";
                    newEnemy.transform.eulerAngles = new Vector3(-90f, 0, 0);
                    //newEnemy.Init(waveData.enemyScriptable);

                    if (waves.ContainsKey(waveData))
                    {
                        waves[waveData].Add(newEnemy);
                    }
                    else
                    {
                        waves.Add(waveData, new List<EnemyStateMachine>() { newEnemy });
                    }

                    //si es un rango que requiere de bullet sumo enemigos de tipo rango
                    var bulletScriptable = waveData.enemyScriptable.TakeBulletScriptable();
                    if (bulletScriptable != null)
                    {
                        totalRangedEnemies++;
                        var bulletData = bulletScriptable.TakeBulletPrefab();
                        if (bulletData.bullet != null && bulletData.count > 0)
                            await bulletPool.CreateBullet(bulletData.bullet, bulletData.count, bulletScriptable);
                    }
                    else
                        totalMeleeEnemies++;


                    float fractionOfCurrentWave = (float)(i + 1) / amountToSpawn;
                    float overallEnemyProgress = (currentWaveIndex + fractionOfCurrentWave) / totalWaves;
                    mapCreator.ReportLocalProgress(overallEnemyProgress);

                    await UniTask.Yield(cancellationToken: ctsCreator.Token);
                }

                currentWaveIndex++;
            }

        }


        private async UniTask SpawnWaveAsync(WaveData waveData, List<EnemyStateMachine> enemies)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                CancellationTokenSource cts = new();
                ctsFindPos.Add(cts);

                try
                {
                    await FindEnemyPosition(waveData, enemies[i], cts);
                    await UniTask.Yield(cts.Token);
                }
                catch (OperationCanceledException) { }
                finally
                {
                    ctsFindPos.Remove(cts);
                    Extensions.ClearCts(ref cts);
                }
            }
        }

        private async UniTask FindEnemyPosition(WaveData waveData, EnemyStateMachine newEnemy, CancellationTokenSource cts)
        {
            int maxAttempts = 20;
            bool placed = false;
            int attempts = 0;
            float enemyRadius = newEnemy.spriteRenderer != null ? newEnemy.spriteRenderer.sprite.bounds.extents.x : 0.5f;
            int cellRadius = Mathf.CeilToInt(enemyRadius);

            while (!placed && attempts < maxAttempts)
            {
                cts.Token.ThrowIfCancellationRequested();

                Vector3 randomWorldPos = GetRandomOffCameraPosition(2f);
                Vector3Int randomCell = mapCreator.groundTilemap.WorldToCell(randomWorldPos);

                randomCell.x = Mathf.Clamp(randomCell.x, mapCreator.margin, mapCreator.currentBiome.size.x - mapCreator.margin);
                randomCell.y = Mathf.Clamp(randomCell.y, mapCreator.margin, mapCreator.currentBiome.size.y - mapCreator.margin);

                Vector3 potentialWorldPos = mapCreator.groundTilemap.GetCellCenterWorld(randomCell);

                if (await mapCreator.IsAreaFree(randomCell, cellRadius, potentialWorldPos))
                {
                    cts.Token.ThrowIfCancellationRequested();

                    if (NavMesh.SamplePosition(potentialWorldPos, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
                    {
                        potentialWorldPos = hit.position;

                        newEnemy.transform.position = potentialWorldPos;
                        newEnemy.gameObject.SetActive(true);

                        newEnemy.agent.enabled = true;
                        newEnemy.agent.Warp(potentialWorldPos);

                        placed = true;
                        newEnemy.Init(waveData.enemyScriptable);
                    }
                }

                attempts++;
            }

            if (!placed)
            {
                Debug.LogWarning($"Could not place enemy after {maxAttempts} attempts.");
            }

        }

        private async UniTask FindPlayerPosition()
        {
            int maxAttempts = 200; // Le damos varios intentos para encontrar el sitio perfecto
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
                bool environmentFree = await mapCreator.IsAreaFree(randomCell, 1, potentialWorldPos);

                if (environmentFree)
                {
                    playerStateMachine.transform.position = potentialWorldPos;
                    placed = true;
                }

                attempts++;
                await UniTask.Yield(cancellationToken: ctsCreator.Token);
            }
        }


        public void ClearNpcs()
        {
            bulletPool.ClearBullets();

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            waves.Clear();
        }

        private Vector3 GetRandomOffCameraPosition(float offsetUnits)
        {
            Camera cam = Camera.main;
            //Vector3 camPos = cam.transform.position;
            Vector3 camPos = playerStateMachine.transform.position;

            float height = cam.orthographicSize;
            float width = height * cam.aspect;

            float minX = camPos.x - width - offsetUnits;
            float maxX = camPos.x + width + offsetUnits;
            float minY = camPos.y - height - offsetUnits;
            float maxY = camPos.y + height + offsetUnits;

            int side = UnityEngine.Random.Range(0, 4);

            switch (side)
            {
                case 0: // Borde Superior
                    return new Vector3(UnityEngine.Random.Range(minX, maxX), maxY, 0);
                case 1: // Borde Derecho
                    return new Vector3(maxX, UnityEngine.Random.Range(minY, maxY), 0);
                case 2: // Borde Inferior
                    return new Vector3(UnityEngine.Random.Range(minX, maxX), minY, 0);
                default: // Borde Izquierdo (case 3)
                    return new Vector3(minX, UnityEngine.Random.Range(minY, maxY), 0);
            }
        }



        private void OnDrawGizmos()
        {
            // Dibujamos el radio de seguridad alrededor del jugador para debug
            if (playerStateMachine != null)
            {
                Gizmos.color = Color.green;
                //Gizmos.DrawWireSphere(playerStateMachine.transform.position, safeDistanceToEnemies);
            }
        }




    }
}
