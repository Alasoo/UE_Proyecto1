using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using MyExtensions;
using System;
using EnemySystem;


namespace GameSystem
{
    public class EnemyCreator : MonoBehaviour
    {
        [Header("ENEMIES")]
        [SerializeField] private List<WaveScriptable> waves = new();

        private CancellationTokenSource ctsCreator;


        private void OnDestroy()
        {
            ctsCreator?.ClearCts();
            ctsCreator = null;
        }


        [ContextMenu("Init")]
        public async UniTask Init()
        {
            ctsCreator?.Cancel();
            ctsCreator = new();
            try
            {
                WaveScriptable wave = waves.RandomElement();
                foreach (WaveData waveData in wave.waveData)
                {
                    int amountToSpawn = UnityEngine.Random.Range(waveData.minEnemies, waveData.maxEnemies + 1);

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
    }
}
