using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "NuevoBioma", menuName = "ScriptableObjects/Bioma", order = 3)]
public class BiomeScriptable : ScriptableObject
{
    public RuleTile waterRule;
    public RuleTile mountainRule;
    public List<Tile> ground = new();

    [Header("MAP SIZE")]
    public Vector2Int size = new(400, 250);

    [Space]
    public int repeatWater = 5;
    public Vector2Int waterSizeMin = new(8, 4);
    public Vector2Int waterSizeMax = new(40, 20);
    [Space]
    public int repeatMountain = 3;
    public Vector2Int mountainSizeMin = new(8, 4);
    public Vector2Int mountainSizeMax = new(40, 20);

    [Space]
    public int treeAmount = 3;
    public List<SpriteRenderer> trees = new();
}


