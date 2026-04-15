using System;
using System.Collections.Generic;
using EnemySystem;
using UnityEngine;

namespace EnemySystem
{
    public abstract class EnemyScriptable : ScriptableObject
    {
        [Header("BASE")]
        public EnemyController enemyControllerPrefab;
        public Sprite sprite;

        [Header("STATS")]
        public float hpMax;
        [Space]
        public float physicalArmor;
        public float magicalArmor;
        public float physicalDamage;
        public float magicalDamage;
        [Space]
        public float speedMov;
        [Space]
        public float rangeAttack;
        public float rangeVision;
        [Space]
        public float patrolRadius;
        [Space]
        public float attackCooldown;
    }
}


