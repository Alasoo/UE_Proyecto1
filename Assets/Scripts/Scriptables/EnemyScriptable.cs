using System;
using System.Collections.Generic;
using BulletSystem;
using Controller.Enemy;
using UnityEngine;


namespace EnemySystem
{
    public abstract class EnemyScriptable : ScriptableObject
    {
        [Header("BASE")]
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


        public virtual EnemyStateMachine TakeCharacterPrefab() { return null; }
        public virtual BulletScriptable TakeBulletScriptable() { return null; }
    }
}


