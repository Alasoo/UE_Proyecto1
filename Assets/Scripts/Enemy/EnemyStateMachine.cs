using System.Collections;
using System.Collections.Generic;
using AttackSystem;
using EnemySystem;
using HealthSystem;
using UnityEngine;
using UnityEngine.AI;


namespace Controller.Enemy
{
    public class EnemyStateMachine : StateMachine
    {
        [field: SerializeField] public NavMeshAgent agent { get; private set; }
        [field: SerializeField] public Transform body { get; private set; }
        [field: SerializeField] public SpriteRenderer spriteRenderer { get; private set; }
        [field: SerializeField] public RangeVision rangeVision { get; private set; }

        [field: Header("UI")]
        [field: SerializeField] public Health health { get; private set; }

        [field: Header("ATTACK")]
        [field: SerializeField] public AttackController attackController { get; private set; }


        public EnemyScriptable enemyScriptable { get; private set; }





        public virtual void Init(EnemyScriptable enemyScriptable)
        {
            this.enemyScriptable = enemyScriptable;
            rangeVision.visionTrigger.radius = enemyScriptable.rangeVision;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            spriteRenderer.sprite = enemyScriptable.sprite;

            rangeVision.OnPlayerEnter += PlayerOnRange;

            //gameObject.SetActive(true);
            health.Init(enemyScriptable.hpMax, spriteRenderer.material);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            rangeVision.OnPlayerEnter -= PlayerOnRange;
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



