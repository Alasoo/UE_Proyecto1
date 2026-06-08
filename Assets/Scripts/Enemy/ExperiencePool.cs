using System.Collections;
using MyExtensions;
using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Controller.Player;
using System.Threading;
using System;

namespace GameSystem
{
    public class ExperiencePool : Singleton<ExperiencePool>
    {
        [SerializeField] private Experience experiencePrefab;

        private int count = 20;
        private Stack<Experience> experienceStack = new();
        private List<CancellationTokenSource> ctsFindPos = new();

        void Start()
        {
            EnemyCreator.Instance.OnDieEnemy += OnDieEnemy;
            CreateExperience();
        }

        void OnDestroy()
        {
            if (EnemyCreator.Instance != null)
                EnemyCreator.Instance.OnDieEnemy -= OnDieEnemy;

            foreach (var cts in ctsFindPos)
            {
                cts?.ClearCts();
            }
            ctsFindPos.Clear();
        }

        public void CreateExperience()
        {
            for (int i = 0; i < count; i++)
            {
                Experience exp = Instantiate(experiencePrefab, transform);
                experienceStack.Push(exp);
                exp.gameObject.SetActive(false);
            }
        }


        private void OnDieEnemy()
        {
            Experience exp = Get();
            _ = SpawnExp(exp);
        }

        private async UniTask SpawnExp(Experience exp)
        {
            CancellationTokenSource cts = new();
            ctsFindPos.Add(cts);
            try
            {
                bool find = await FindExperiencePosition(exp, cts.Token);
                if (!find) return;
                exp.gameObject.SetActive(true);
            }
            catch (OperationCanceledException) { }
            finally
            {
                ctsFindPos.Remove(cts);
            }
        }





        public Experience Get()
        {
            Experience exp;
            if (experienceStack.Count > 0)
            {
                exp = experienceStack.Pop();
            }
            else
            {
                exp = Instantiate(experiencePrefab, transform);
            }

            return exp;
        }

        public void Return(Experience experience)
        {
            experienceStack.Push(experience);
            experience.gameObject.SetActive(false);
        }






        private async UniTask<bool> FindExperiencePosition(Experience exp, CancellationToken cts)
        {
            int attemptsPerDistance = 10;
            int maxDistanceSteps = 12;

            float startOffset = 2f;
            float offsetIncrease = 2f;

            float enemyRadius = exp.spriteRenderer != null ? exp.spriteRenderer.sprite.bounds.extents.x : 0.5f;
            int cellRadius = Mathf.CeilToInt(enemyRadius);

            for (int distanceStep = 0; distanceStep < maxDistanceSteps; distanceStep++)
            {
                float currentOffset = startOffset + offsetIncrease * distanceStep;

                for (int attempt = 0; attempt < attemptsPerDistance; attempt++)
                {
                    cts.ThrowIfCancellationRequested();

                    Vector3 randomWorldPos = GetRandomOffCameraPosition(currentOffset);
                    Vector3Int randomCell = MapCreator.Instance.groundTilemap.WorldToCell(randomWorldPos);

                    if (!IsCellInsideSpawnBounds(randomCell))
                    {
                        await UniTask.Yield(cts);
                        continue;
                    }

                    Vector3 potentialWorldPos = MapCreator.Instance.groundTilemap.GetCellCenterWorld(randomCell);

                    if (!await MapCreator.Instance.IsAreaFree(randomCell, cellRadius, potentialWorldPos))
                    {
                        await UniTask.Yield(cts);
                        continue;
                    }

                    exp.transform.position = potentialWorldPos;
                    return true;
                }
            }

            Debug.LogWarning($"Could not place enemy after {attemptsPerDistance * maxDistanceSteps} attempts.");

            return false;
        }

        private Vector3 GetRandomOffCameraPosition(float offsetUnits)
        {
            Camera cam = Camera.main;
            Vector3 camPos = PlayerStateMachine.Instance.transform.position;

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


        private bool IsCellInsideSpawnBounds(Vector3Int cell)
        {
            return cell.x >= MapCreator.Instance.margin &&
                   cell.x < MapCreator.Instance.currentBiome.size.x - MapCreator.Instance.margin &&
                   cell.y >= MapCreator.Instance.margin &&
                   cell.y < MapCreator.Instance.currentBiome.size.y - MapCreator.Instance.margin;
        }


    }
}