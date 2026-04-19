
using System.Collections.Generic;
using BulletSystem;
using EnemySystem;
using UnityEngine;

namespace AttackSystem
{
    public abstract class AttackController : MonoBehaviour
    {
        public abstract void Attack(EnemyScriptable enemyScriptable, Vector3 direction, Vector3 position);
        public abstract void StopAttack();
    }
}