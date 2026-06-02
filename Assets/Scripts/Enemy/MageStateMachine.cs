using System.Collections;
using System.Collections.Generic;
using EnemySystem;
using UnityEngine;
using UnityEngine.UI;


namespace Controller.Enemy
{
    public class MageStateMachine : EnemyStateMachine
    {
        [field: Header("UI")]
        [SerializeField] private Slider manaSlider;




        public override void Init(EnemyScriptable enemyScriptable)
        {
            base.Init(enemyScriptable);
            manaSlider.gameObject.SetActive(false);
            SwitchState(new EnemyFollowPlayerState(this));
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



