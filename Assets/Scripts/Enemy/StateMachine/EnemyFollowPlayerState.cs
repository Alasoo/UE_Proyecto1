using System.Collections;
using UnityEngine;
using Controller.Player;


namespace Controller.Enemy
{
    public class EnemyFollowPlayerState : EnemyBaseState
    {
        public EnemyFollowPlayerState(EnemyStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter() { }

        public override void Tick(float deltaTime)
        {
            stateMachine.agent.SetDestination(PlayerStateMachine.Instance.transform.position);
            /*
            if (Vector3.Distance(PlayerStateMachine.Instance.transform.position, stateMachine.transform.position) <= stateMachine.enemyScriptable.rangeAttack)
            {
                stateMachine.SwitchState(new EnemyAttackState(stateMachine));
            }
            */
        }

        public override void PlayerOnRange(bool inRange)
        {
            if (inRange)
                stateMachine.SwitchState(new EnemyAttackState(stateMachine));
        }

        public override void LateTick(float deltaTime)
        {
            RotateBodyTowardsMovement(deltaTime);
        }

        public override void Exit()
        {
            stateMachine.agent.ResetPath();
        }

    }
}