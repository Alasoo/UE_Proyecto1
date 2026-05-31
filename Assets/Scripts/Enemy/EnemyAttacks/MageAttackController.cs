
using System.Collections.Generic;
using AudioController;
using BulletSystem;
using EnemySystem;
using UnityEngine;

namespace AttackSystem
{
    public class MageAttackController : AttackController
    {
        public override void Attack(EnemyScriptable enemyScriptable, Vector3 direction, Vector3 position)
        {
            var bulletScriptable = enemyScriptable.TakeBulletScriptable(); //.TakeBulletPrefab();
            var bulletData = bulletScriptable.TakeBulletPrefab();
            BulletPool.Instance.Get(bulletData.bullet, direction, position, bulletScriptable);
            Audios.Instance.PlayEffect(soundAttack);
        }

        public override void StopAttack()
        {
            Debug.Log($"Stop ataque!");
        }




    }
}