using System;
using System.Collections.Generic;
using EnemySystem;
using UnityEngine;

namespace EnemySystem
{
    [CreateAssetMenu(fileName = "NewMeleeEnemy", menuName = "ScriptableObjects/Enemies/Melee", order = 1)]
    public class MeleeEnemyScriptable : EnemyScriptable
    {
        [Header("Melee Combat")]
        public float energyMax;
    }
}


