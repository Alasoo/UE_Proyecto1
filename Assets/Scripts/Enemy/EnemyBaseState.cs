using System.Collections;
using System.Collections.Generic;
using Controller.Player;
using UnityEngine;


namespace Controller.Enemy
{
    public class EnemyBaseState : State
    {
        protected EnemyStateMachine stateMachine;

        private float rotationSpeed = 10f;

        public EnemyBaseState(EnemyStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }



        protected void RotateBodyTowardsMovement(float deltaTime)
        {
            if (stateMachine.agent.velocity.magnitude <= 0f) return;
            float angle = Mathf.Atan2(stateMachine.agent.velocity.y, stateMachine.agent.velocity.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            stateMachine.body.rotation = Quaternion.Slerp(stateMachine.body.rotation, targetRotation, rotationSpeed * deltaTime);
        }

        protected void RotateBodyTowardsPlayer(float deltaTime)
        {
            Vector3 direction = PlayerStateMachine.Instance.transform.position - stateMachine.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            stateMachine.body.rotation = Quaternion.Slerp(stateMachine.body.rotation, targetRotation, rotationSpeed * deltaTime);
        }

    }
}
