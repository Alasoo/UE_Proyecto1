using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyExtensions;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameSystem
{
    public class MapCreator : Singleton<MapCreator>
    {
        [Header("REFERENCES")]
        [field: SerializeField] public Tilemap groundTilemap { get; private set; }
        [SerializeField] private Tilemap waterTilemap;
        [SerializeField] private Tilemap mountainTilemap;

        [Header("BIOMES")]
        [SerializeField] private List<BiomeScriptable> biomeList = new();

        [Header("MARGIN LIMIT")]
        [field: SerializeField] public int margin { get; private set; } = 10;
        [Header("MARGIN BETWEEN TREES")]
        [SerializeField] float treeSpacing = 1.5f;


        [Header("NAV MESH")]
        [SerializeField] private NavMeshSurface navMeshSurface;
        [Header("ENEMY CREATOR")]
        [SerializeField] private EnemyCreator enemyCreator;



        private CancellationTokenSource ctsCreator, ctsAreaFree;

        private List<Vector3Int> mountainTileUsed = new();
        private List<Vector3Int> waterTileUsed = new();
        private Dictionary<SpriteRenderer, Vector3> treeList = new();


        public event Action<float> OnProgress;
        private float baseProgress = 0f;
        private float currentStepWeight = 0f;

        public BiomeScriptable currentBiome { get; private set; }

        private void OnDestroy()
        {
            Extensions.ClearCts(ref ctsCreator);
            Extensions.ClearCts(ref ctsAreaFree);
        }

        protected override void Awake()
        {
            base.Awake();
            //_ = Init();
        }

        [ContextMenu("Init")]
        public async UniTask Init()
        {
            ctsCreator?.Cancel();
            ctsCreator = new();
            try
            {
                float time = Time.time;
                ClearMap();
                enemyCreator.ClearNpcs();

                currentBiome = biomeList.RandomElement();

                baseProgress = 0f;

                currentStepWeight = 0.15f;
                await MakeMountains(currentBiome);
                baseProgress += currentStepWeight;

                currentStepWeight = 0.20f;
                await MakeGround(currentBiome);
                baseProgress += currentStepWeight;

                currentStepWeight = 0.10f;
                await MakeWater(currentBiome);
                baseProgress += currentStepWeight;

                currentStepWeight = 0.15f;
                await MakeTrees(currentBiome);
                baseProgress += currentStepWeight;

                currentStepWeight = 0.15f;
                await MakeLimit(currentBiome);
                baseProgress += currentStepWeight;

                currentStepWeight = 0.05f;
                await navMeshSurface.BuildNavMeshAsync();
                ReportLocalProgress(1f);
                baseProgress += currentStepWeight;

                await UniTask.Yield(ctsCreator.Token);

                currentStepWeight = 0.20f;
                await enemyCreator.Init();
                baseProgress += currentStepWeight;

                baseProgress = 1f;
                OnProgress?.Invoke(baseProgress);
                GameManager.Instance.StartGame();
                Debug.Log($"Tiempo en crear bioma: {Time.time - time}");
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
                Extensions.ClearCts(ref ctsCreator);
            }
        }

        private void ClearMap()
        {
            groundTilemap.ClearAllTiles();
            waterTilemap.ClearAllTiles();
            mountainTilemap.ClearAllTiles();
            // Elimina todos los objetos que tenga como hijo este GameObject
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            treeList.Clear();
        }

        private async UniTask MakeMountains(BiomeScriptable biome)
        {
            for (int i = 0; i < biome.repeatMountain; i++)
            {
                ctsCreator.Token.ThrowIfCancellationRequested();
                int startRandomPosX = UnityEngine.Random.Range(margin, biome.size.x);
                int startRandomPosY = UnityEngine.Random.Range(margin, biome.size.y);

                int mountainWidth = UnityEngine.Random.Range(biome.mountainSizeMin.x, biome.mountainSizeMax.x);
                int mountainHeight = UnityEngine.Random.Range(biome.mountainSizeMin.y, biome.mountainSizeMax.y);

                int currentMountainTilesTotal = mountainWidth * mountainHeight;
                int currentMountainTileCount = 0;

                for (int x = 0; x < mountainWidth; x++)
                {
                    ctsCreator.Token.ThrowIfCancellationRequested();
                    Vector3Int currentTileX = new(startRandomPosX + x, startRandomPosY, 0);
                    mountainTilemap.SetTile(currentTileX, biome.mountainRule);
                    mountainTileUsed.Add(currentTileX);

                    for (int y = 0; y < mountainHeight; y++)
                    {
                        ctsCreator.Token.ThrowIfCancellationRequested();
                        Vector3Int currentTileY = new(startRandomPosX + x, startRandomPosY + y, 0);
                        mountainTilemap.SetTile(currentTileY, biome.mountainRule);
                        mountainTileUsed.Add(currentTileY);

                        currentMountainTileCount++;
                        float fractionOfCurrentMountain = (float)currentMountainTileCount / currentMountainTilesTotal;
                        float overallMountainProgress = (i + fractionOfCurrentMountain) / biome.repeatMountain;
                        ReportLocalProgress(overallMountainProgress);
                    }
                }
                await UniTask.Yield(cancellationToken: ctsCreator.Token);
            }
        }

        private async UniTask MakeGround(BiomeScriptable biome)
        {
            int width = biome.size.x - (margin * 2);
            int height = biome.size.y - (margin * 2);
            int total = width * height;
            int currentProgress = 0;

            for (int x = margin; x < biome.size.x - margin; x++)
            {
                for (int y = margin; y < biome.size.y - margin; y++)
                {
                    ctsCreator.Token.ThrowIfCancellationRequested();
                    Vector3Int currentTile = new Vector3Int(x, y, 0);
                    groundTilemap.SetTile(currentTile, biome.ground[UnityEngine.Random.Range(0, biome.ground.Count)]);
                    currentProgress++;
                    ReportLocalProgress((float)currentProgress / total);
                }
                await UniTask.Yield(cancellationToken: ctsCreator.Token);
            }
        }

        private async UniTask MakeWater(BiomeScriptable biome)
        {
            for (int i = 0; i < biome.repeatWater; i++)
            {
                ctsCreator.Token.ThrowIfCancellationRequested();

                int attemps = 100;
                Vector3Int startWaterPos = new(UnityEngine.Random.Range(0, biome.size.x), UnityEngine.Random.Range(0, biome.size.y), 0);

                while (mountainTileUsed.Contains(startWaterPos))
                {
                    ctsCreator.Token.ThrowIfCancellationRequested();
                    if (attemps <= 0)
                    {
                        Debug.LogError($"No se ha podido encontrar sitio para el agua");
                        break;
                    }
                    startWaterPos = new(UnityEngine.Random.Range(0, biome.size.x), UnityEngine.Random.Range(0, biome.size.y), 0);
                    attemps--;
                }

                int waterWidth = UnityEngine.Random.Range(biome.waterSizeMin.x, biome.waterSizeMax.x);
                int waterHeight = UnityEngine.Random.Range(biome.waterSizeMin.y, biome.waterSizeMax.y);

                int currentWaterTilesTotal = waterWidth * waterHeight;
                int currentWaterTileCount = 0;

                for (int x = 0; x < waterWidth; x++)
                {
                    ctsCreator.Token.ThrowIfCancellationRequested();

                    Vector3Int currentTileX = new(startWaterPos.x + x, startWaterPos.y, 0);
                    if (currentTileX.x > biome.size.x) continue;
                    waterTilemap.SetTile(currentTileX, biome.waterRule);
                    waterTileUsed.Add(currentTileX);
                    for (int y = 0; y < waterHeight; y++)
                    {
                        ctsCreator.Token.ThrowIfCancellationRequested();
                        Vector3Int currentTileY = new(startWaterPos.x + x, startWaterPos.y + y, 0);
                        if (currentTileY.y > biome.size.y) continue;
                        waterTilemap.SetTile(currentTileY, biome.waterRule);
                        waterTileUsed.Add(currentTileY);

                        currentWaterTileCount++;
                        float fractionOfCurrentWater = (float)currentWaterTileCount / currentWaterTilesTotal;
                        float overallWaterProgress = (i + fractionOfCurrentWater) / biome.repeatWater;
                        ReportLocalProgress(overallWaterProgress);
                    }
                }
                await UniTask.Yield(cancellationToken: ctsCreator.Token);
            }
        }

        private async UniTask MakeTrees(BiomeScriptable biome)
        {
            if (biome.trees.Count == 0) return;

            // Limite de intentos para que el juego no se cuelgue si el mapa está muy lleno
            int maxAttempts = 20;

            for (int i = 0; i < biome.treeAmount; i++)
            {
                ctsCreator.Token.ThrowIfCancellationRequested();

                SpriteRenderer sr = biome.trees[UnityEngine.Random.Range(0, biome.trees.Count)];
                float treeRadius = sr != null ? sr.sprite.bounds.extents.x : 0.5f;
                int cellRadius = Mathf.CeilToInt(treeRadius);

                bool placed = false;
                int attempts = 0;

                while (!placed && attempts < maxAttempts)
                {
                    ctsCreator.Token.ThrowIfCancellationRequested();

                    // Elegimos una coordenada aleatoria dentro del mapa (respetando el margen)
                    Vector3Int randomCell = new Vector3Int(
                        UnityEngine.Random.Range(margin, biome.size.x - margin),
                        UnityEngine.Random.Range(margin, biome.size.y - margin),
                        0
                    );
                    Vector3 potentialWorldPos = groundTilemap.GetCellCenterWorld(randomCell);

                    // Comprobamos si el área entera que ocupará el árbol está libre
                    if (await IsAreaFree(randomCell, cellRadius, potentialWorldPos))
                    {
                        // Convertimos la coordenada del Grid a posición real del mundo
                        Vector3 worldPos = groundTilemap.GetCellCenterWorld(randomCell);

                        // Instanciamos el árbol y lo emparentamos a este objeto para mantener la jerarquía limpia
                        SpriteRenderer sp = Instantiate(sr, worldPos, Quaternion.identity, this.transform);
                        placed = true;
                        treeList.Add(sp, worldPos);
                    }

                    attempts++;
                }
                ReportLocalProgress((float)(i + 1) / biome.treeAmount);
                await UniTask.Yield(cancellationToken: ctsCreator.Token);
            }
        }

        public async UniTask<bool> IsAreaFree(Vector3Int centerCell, int radius, Vector3 potentialPos)
        {
            ctsAreaFree?.Cancel();
            ctsAreaFree = new();
            
            foreach (Vector3 existingTreePos in treeList.Values)
            {
                ctsAreaFree.Token.ThrowIfCancellationRequested();
                if (Vector3.Distance(potentialPos, existingTreePos) < treeSpacing)
                {
                    return false; // Está demasiado cerca de otro árbol
                }
            }
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    ctsAreaFree.Token.ThrowIfCancellationRequested();
                    Vector3Int checkPos = new Vector3Int(centerCell.x + x, centerCell.y + y, 0);
                    if (mountainTilemap.HasTile(checkPos) || waterTilemap.HasTile(checkPos))
                    {
                        return false;
                    }
                }
                await UniTask.Yield(cancellationToken: ctsAreaFree.Token);
            }
            return true;
        }

        private async UniTask MakeLimit(BiomeScriptable biome)
        {
            int total = biome.size.x * biome.size.y;
            int currentProgress = 0;

            for (int x = 0; x < biome.size.x; x++)
            {
                for (int y = 0; y < biome.size.y; y++)
                {
                    ctsCreator.Token.ThrowIfCancellationRequested();

                    if (x < margin || x >= biome.size.x - margin ||
                        y < margin || y >= biome.size.y - margin)
                    {
                        Vector3Int currentTile = new Vector3Int(x, y, 0);
                        mountainTilemap.SetTile(currentTile, biome.mountainRule);

                        if (!mountainTileUsed.Contains(currentTile))
                            mountainTileUsed.Add(currentTile);
                    }

                    currentProgress++;
                    ReportLocalProgress((float)currentProgress / total);
                }
                await UniTask.Yield(cancellationToken: ctsCreator.Token);
            }
        }

        public void ReportLocalProgress(float localProgress)
        {
            OnProgress?.Invoke(baseProgress + (currentStepWeight * localProgress));
        }
    }
}
