using System;
using System.Collections.Generic;
using BulletSystem;
using Controller.Enemy;
using UnityEngine;

namespace EnemySystem
{
    [CreateAssetMenu(fileName = "NewMageEnemy", menuName = "ScriptableObjects/Enemies/Mage", order = 1)]
    public class MageEnemyScriptable : EnemyScriptable
    {
        [Header("MAGE COMBAT")]
        public MageStateMachine enemyPrefab;
        public float manaMax;

        [Header("BULLETS")]
        public MagicBulletScriptable bulletScriptable;



        public override BulletScriptable TakeBulletScriptable()
        {
            return bulletScriptable;
        }
        public override EnemyStateMachine TakeCharacterPrefab()
        {
            return enemyPrefab;
        }
        /*
        public override (Bullet bullet, int count) TakeBulletPrefab()
        {
            return (bulletPrefab, bulletCount);
        }
        */
    }
}


