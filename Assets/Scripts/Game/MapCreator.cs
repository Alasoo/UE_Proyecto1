using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapCreator : MonoBehaviour
{

    [Header("REFERENCES")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap waterTilemap;
    [SerializeField] private Tilemap mountainTilemap;



    [Header("BIOMES")]
    [SerializeField] private List<BiomeScriptable> biomeList = new();

    [Header("MARGIN LIMIT")]
    [SerializeField] private int margin = 20;
    [Header("MARGIN BETWEEN TREES")]
    [SerializeField] float treeSpacing = 1.5f;



    private List<Vector3Int> mountainTileUsed = new();
    private List<Vector3Int> waterTileUsed = new();
    private Dictionary<SpriteRenderer, Vector3> treeList = new();

    private void Awake()
    {
        BiomeScriptable randomBiome = biomeList[UnityEngine.Random.Range(0, biomeList.Count)];

        MakeMountains(randomBiome);
        MakeGround(randomBiome);
        MakeWater(randomBiome);
        MakeTrees(randomBiome);
    }

    [ContextMenu("Create map")]
    private void Test()
    {
        ClearMap();
        BiomeScriptable randomBiome = biomeList[UnityEngine.Random.Range(0, biomeList.Count)];

        MakeMountains(randomBiome);
        MakeLimit(randomBiome);
        MakeGround(randomBiome);
        MakeWater(randomBiome);
        MakeTrees(randomBiome);
    }

    [ContextMenu("Clear map")]
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


    private void MakeMountains(BiomeScriptable biome)
    {
        for (int i = 0; i < biome.repeatMountain; i++)
        {
            int startRandomPosX = Random.Range(margin, biome.size.x);
            int startRandomPosY = Random.Range(margin, biome.size.y);

            int mountainWidth = Random.Range(biome.mountainSizeMin.x, biome.mountainSizeMax.x);
            int mountainHeight = Random.Range(biome.mountainSizeMin.y, biome.mountainSizeMax.y);
            for (int x = 0; x < mountainWidth; x++)
            {
                Vector3Int currentTileX = new(startRandomPosX + x, startRandomPosY, 0);
                mountainTilemap.SetTile(currentTileX, biome.mountainRule);
                mountainTileUsed.Add(currentTileX);

                for (int y = 0; y < mountainHeight; y++)
                {
                    Vector3Int currentTileY = new(startRandomPosX + x, startRandomPosY + y, 0);
                    mountainTilemap.SetTile(currentTileY, biome.mountainRule);
                    mountainTileUsed.Add(currentTileY);
                }
            }

        }
    }

    [ContextMenu("Make Ground")]
    private void MakeGround()
    {
        groundTilemap.ClearAllTiles();
        BiomeScriptable randomBiome = biomeList[UnityEngine.Random.Range(0, biomeList.Count)];
        MakeGround(randomBiome);
    }

    [ContextMenu("Make Limit")]
    private void MakeLimit()
    {
        mountainTilemap.ClearAllTiles();
        BiomeScriptable randomBiome = biomeList[UnityEngine.Random.Range(0, biomeList.Count)];
        MakeLimit(randomBiome);
    }

    private void MakeGround(BiomeScriptable biome)
    {
        for (int x = margin; x < biome.size.x - margin; x++)
        {
            for (int y = margin; y < biome.size.y - margin; y++)
            {
                Vector3Int currentTile = new Vector3Int(x, y, 0);
                groundTilemap.SetTile(currentTile, biome.ground[Random.Range(0, biome.ground.Count)]);
            }
        }
    }

    private void MakeWater(BiomeScriptable biome)
    {
        for (int i = 0; i < biome.repeatWater; i++)
        {
            int attemps = 100;
            Vector3Int startWaterPos = new(Random.Range(0, biome.size.x), Random.Range(0, biome.size.y), 0);

            while (mountainTileUsed.Contains(startWaterPos))
            {
                if (attemps <= 0)
                {
                    Debug.LogError($"No se ha podido encontrar sitio para el agua");
                    break;
                }
                startWaterPos = new(Random.Range(0, biome.size.x), Random.Range(0, biome.size.y), 0);
                attemps--;
            }

            int waterWidth = Random.Range(biome.waterSizeMin.x, biome.waterSizeMax.x);
            int waterHeight = Random.Range(biome.waterSizeMin.y, biome.waterSizeMax.y);
            for (int x = 0; x < waterWidth; x++)
            {
                Vector3Int currentTileX = new(startWaterPos.x + x, startWaterPos.y, 0);
                if (currentTileX.x > biome.size.x) continue;
                waterTilemap.SetTile(currentTileX, biome.waterRule);
                waterTileUsed.Add(currentTileX);
                for (int y = 0; y < waterHeight; y++)
                {
                    Vector3Int currentTileY = new(startWaterPos.x + x, startWaterPos.y + y, 0);
                    if (currentTileY.y > biome.size.y) continue;
                    waterTilemap.SetTile(currentTileY, biome.waterRule);
                    waterTileUsed.Add(currentTileY);
                }
            }
        }
    }



    private void MakeLimit(BiomeScriptable biome)
    {
        for (int x = 0; x < biome.size.x; x++)
        {
            for (int y = 0; y < biome.size.y; y++)
            {
                if (x < margin || x >= biome.size.x - margin ||
                    y < margin || y >= biome.size.y - margin)
                {
                    Vector3Int currentTile = new Vector3Int(x, y, 0);
                    mountainTilemap.SetTile(currentTile, biome.mountainRule);

                    if (!mountainTileUsed.Contains(currentTile))
                        mountainTileUsed.Add(currentTile);
                }
            }
        }
    }


    private void MakeTrees(BiomeScriptable biome)
    {
        if (biome.trees.Count == 0) return;
        // Límite de intentos para que el juego no se cuelgue si el mapa está muy lleno
        int maxAttempts = 20;

        for (int i = 0; i < biome.treeAmount; i++)
        {
            SpriteRenderer sr = biome.trees[Random.Range(0, biome.trees.Count)];

            // Calculamos cuánto ocupa el árbol leyendo el tamaño de su Sprite.
            // Si prefieres usar un CircleCollider2D, sería: treePrefab.GetComponent<CircleCollider2D>().radius
            float treeRadius = sr != null ? sr.sprite.bounds.extents.x : 0.5f;

            // Convertimos ese tamaño en unidades de Unity a "cantidad de celdas"
            int cellRadius = Mathf.CeilToInt(treeRadius);

            bool placed = false;
            int attempts = 0;

            while (!placed && attempts < maxAttempts)
            {
                // Elegimos una coordenada aleatoria dentro del mapa (respetando el margen)
                Vector3Int randomCell = new Vector3Int(
                    Random.Range(margin, biome.size.x - margin),
                    Random.Range(margin, biome.size.y - margin),
                    0
                );
                Vector3 potentialWorldPos = groundTilemap.GetCellCenterWorld(randomCell);

                // Comprobamos si el área entera que ocupará el árbol está libre
                if (IsAreaFree(randomCell, cellRadius, potentialWorldPos))
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
        }
    }

    private bool IsAreaFree(Vector3Int centerCell, int radius, Vector3 potentialPos)
    {

        foreach (Vector3 existingTreePos in treeList.Values)
        {
            if (Vector3.Distance(potentialPos, existingTreePos) < treeSpacing)
            {
                return false; // Está demasiado cerca de otro árbol
            }
        }
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int checkPos = new Vector3Int(centerCell.x + x, centerCell.y + y, 0);
                if (mountainTilemap.HasTile(checkPos) || waterTilemap.HasTile(checkPos))
                {
                    return false;
                }
            }
        }
        return true;
    }



}
