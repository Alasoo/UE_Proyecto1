
using System;
using System.Collections.Generic;

using EnemySystem;
using UnityEngine;

namespace AttackSystem
{
    public abstract class AttackController : MonoBehaviour
    {
        [SerializeField] protected AudioClip soundAttack;
        public abstract void Attack(EnemyScriptable enemyScriptable, Vector3 direction, Vector3 position);
        public abstract void StopAttack();


    }
}