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
            // PARCHE DE SEGURIDAD: 
            // Si el enemigo está "reciclado" (apagado o fuera del mapa), no intentamos moverlo.
            if (!stateMachine.agent.isActiveAndEnabled || !stateMachine.agent.isOnNavMesh) return;

            stateMachine.agent.SetDestination(PlayerStateMachine.Instance.transform.position);
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
            // Aplicamos el mismo freno de seguridad aquí por si acaso se apaga justo al cambiar de estado
            if (!stateMachine.agent.isActiveAndEnabled || !stateMachine.agent.isOnNavMesh) return;

            stateMachine.agent.ResetPath();
        }
    }
}