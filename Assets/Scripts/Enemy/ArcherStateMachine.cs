using System.Collections;
using System.Collections.Generic;
using EnemySystem;
using UnityEngine;
using UnityEngine.UI;


namespace Controller.Enemy
{
    public class ArcherStateMachine : EnemyStateMachine
    {
        [field: Header("UI")]
        [SerializeField] private Slider energySlider;




        public override void Init(EnemyScriptable enemyScriptable)
        {
            base.Init(enemyScriptable);
            energySlider.gameObject.SetActive(true);
            SwitchState(new EnemyPatrollState(this));
        }



        void OnDrawGizmosSelected()
        {
            if (enemyScriptable == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyScriptable.patrolRadius);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, enemyScriptable.rangeVision);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, enemyScriptable.rangeAttack);
        }



    }
}



