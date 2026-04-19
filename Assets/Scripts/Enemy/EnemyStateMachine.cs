using System.Collections;
using System.Collections.Generic;
using AttackSystem;
using EnemySystem;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


namespace Controller.Enemy
{
    public class EnemyStateMachine : StateMachine
    {
        [field: SerializeField] public NavMeshAgent agent { get; private set; }
        [field: SerializeField] public Transform body { get; private set; }
        [field: SerializeField] public SpriteRenderer spriteRenderer { get; private set; }
        [field: SerializeField] public CircleCollider2D visionTrigger { get; private set; }

        [field: Header("UI")]
        [field: SerializeField] public Canvas canvas { get; private set; }
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TMP_Text levelText;

        [field: Header("ATTACK")]
        [field: SerializeField] public AttackController attackController { get; private set; }


        public EnemyScriptable enemyScriptable { get; private set; }





        public virtual void Init(EnemyScriptable enemyScriptable)
        {
            this.enemyScriptable = enemyScriptable;
            visionTrigger.radius = enemyScriptable.rangeVision;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            spriteRenderer.sprite = enemyScriptable.sprite;

            gameObject.SetActive(true);
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



