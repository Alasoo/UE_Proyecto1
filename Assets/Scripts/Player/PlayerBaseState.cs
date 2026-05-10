using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller.Player
{
    public class PlayerBaseState : State
    {

        protected PlayerStateMachine stateMachine;
        private float turnSmoothVelocity;

        public PlayerBaseState(PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }


        protected void Move(float deltaTime, Vector2 direction)
        {
            //float speed = stateMachine.inputReader.sprint ? stateMachine.sprintSpeed : stateMachine.moveSpeed;
            Vector2 targetVelocity = direction * stateMachine.playerStats.GetMovementSpeed;
            //Debug.Log($"targetVelocity: {targetVelocity}");
            stateMachine.rb.linearVelocity = new Vector2(targetVelocity.x, targetVelocity.y);
        }

        protected void Rotate(Vector2 direction)
        {
            float targetAngle = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg;
            targetAngle -= 90f;
            float angle = Mathf.SmoothDampAngle(stateMachine.transform.eulerAngles.z, targetAngle, ref turnSmoothVelocity, stateMachine.turnSmoothTime);
            stateMachine.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }


        protected Vector2 Direction()
        {
            Vector2 camUp = stateMachine.mainCamera.transform.up;
            Vector2 camRight = stateMachine.mainCamera.transform.right;

            Vector2 moveDirection = (camUp * stateMachine.inputReader.movementValue.y + camRight * stateMachine.inputReader.movementValue.x).normalized;
            return moveDirection;
        }

    }
}
