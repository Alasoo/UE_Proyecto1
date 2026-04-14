using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NuevoEnemigo", menuName = "ScriptableObjects/Enemigo", order = 4)]
public class EnemyScriptable : ScriptableObject
{
    public SpriteRenderer sprite;
    public EnemyData enemyData;
    
}

[Serializable]
public struct EnemyData
{
    public float hpMax;
    public float manaMax;
    public float energyMax;

    public float physicalArmor;
    public float magicalArmor;
    public float physicalDamage;
    public float magicalDamage;
    public float speedMov;
}

