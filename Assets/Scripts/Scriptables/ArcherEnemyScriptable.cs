using System;
using System.Collections.Generic;
using BulletSystem;
using Controller.Enemy;
using UnityEngine;

namespace EnemySystem
{
    [CreateAssetMenu(fileName = "NewArcherEnemy", menuName = "ScriptableObjects/Enemies/Archer", order = 1)]
    public class ArcherEnemyScriptable : EnemyScriptable
    {
        [Header("MAGE COMBAT")]
        public ArcherStateMachine enemyPrefab;
        public float energyMax;

        [Header("BULLETS")]
        public BulletScriptable bulletScriptable;



        public override BulletScriptable TakeBulletScriptable()
        {
            return bulletScriptable;
        }
        public override EnemyStateMachine TakeCharacterPrefab()
        {
            return enemyPrefab;
        }
    }
}


