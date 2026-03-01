using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Controller.Player
{
    public class PlayerBaseState : State
    {

        protected PlayerStateMachine stateMachine;

        public PlayerBaseState(PlayerStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        /*
        protected void Move(float deltaTime)
        {
            Move(Vector3.zero, deltaTime);
        }
        */


        protected void Move(float deltaTime, Vector3 direction)
        {
            float speed = stateMachine.inputReader.sprint ? stateMachine.sprintSpeed : stateMachine.moveSpeed;

            // Calculamos la velocidad objetivo en los ejes X y Z
            Vector3 targetVelocity = direction * speed;

            // En Unity 6 usamos 'linearVelocity' en lugar del obsoleto 'velocity'.
            // Mantenemos la velocidad Y actual para que la gravedad y el salto sigan funcionando.
            stateMachine.rb.linearVelocity = new Vector3(targetVelocity.x, stateMachine.rb.linearVelocity.y, targetVelocity.z);
            /*
            //stateMachine.controller.Move((motion + stateMachine.forceReceiver.Movement) * deltaTime);


            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = stateMachine.inputReader.sprint ? stateMachine.sprintSpeed : stateMachine.moveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (stateMachine.inputReader.movementValue == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(stateMachine.rb.linearVelocity.x, 0.0f, stateMachine.rb.linearVelocity.z).magnitude;

            float speedOffset = 0.1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                stateMachine.speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed ,
                    deltaTime * stateMachine.speedChangeRate);

                // round speed to 3 decimal places
                stateMachine.speed = Mathf.Round(stateMachine.speed * 1000f) / 1000f;
            }
            else
            {
                stateMachine.speed = targetSpeed;
            }
*/


            // normalise input direction
            //Vector3 inputDirection = new Vector3(stateMachine.inputReader.movementValue.x, 0.0f, stateMachine.inputReader.move.y).normalized;

            //Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;


            // move the player
            //_controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
            //                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);


        }

        private Vector3 ForwardCamera()
        {
            Vector3 forward = stateMachine.mainCamera.transform.forward.normalized;
            forward.y = 0f;


            return forward;
        }

        protected Vector3 Direction()
        {
            // Obtenemos las direcciones de la cámara
            Vector3 camForward = stateMachine.mainCamera.transform.forward;
            Vector3 camRight = stateMachine.mainCamera.transform.right;

            // Aplanamos los vectores para que el personaje no se mueva hacia arriba/abajo
            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            // Calculamos la dirección de movimiento basada en los inputs y la cámara
            Vector3 moveDirection = (camForward * stateMachine.inputReader.movementValue.y + camRight * stateMachine.inputReader.movementValue.x).normalized;

            return moveDirection;
        }

        protected void GroundedCheck()
        {
            stateMachine.grounded = Physics.CheckSphere(stateMachine.groundCheck.position, stateMachine.groundedRadius, stateMachine.groundLayers);

            if (!stateMachine.grounded && stateMachine.isLeavingGround)
            {
                stateMachine.isLeavingGround = false; // Liberamos el candado para el próximo aterrizaje
            }
            //stateMachine.isGrounded = Physics.CheckSphere(stateMachine.groundCheck.position, stateMachine.groundCheckRadius);
        }

        protected void Jump()
        {
            stateMachine.jumpRequested = true;
        }

        protected void JumpEffect()
        {
            if (stateMachine.jumpRequested)
            {
                // Verificamos que esté en el suelo y que no esté en medio de salir de él
                if (stateMachine.grounded && !stateMachine.isLeavingGround)
                {
                    // Reseteamos la velocidad en Y antes de saltar para que el salto siempre tenga la misma altura
                    stateMachine.rb.linearVelocity = new Vector3(stateMachine.rb.linearVelocity.x, 0f, stateMachine.rb.linearVelocity.z);

                    stateMachine.rb.AddForce(Vector3.up * stateMachine.jumpForce, ForceMode.Impulse);

                    stateMachine.isLeavingGround = true; // Ponemos el candado hasta que salgamos del suelo
                }

                // Consumimos el input para que no se "guarde" si el jugador pulsó salto en el aire
                stateMachine.jumpRequested = false;
            }
        }

        protected void FallingEffect()
        {
            // Si el personaje está cayendo (velocidad en Y negativa)
            if (stateMachine.rb.linearVelocity.y < 0)
            {
                // Aplicamos una fuerza adicional hacia abajo usando la gravedad general de Unity
                stateMachine.rb.AddForce(Physics.gravity * (stateMachine.fallMultiplier - 1f), ForceMode.Acceleration);
            }
        }

        protected Vector3 CalculateMovement()
        {
            Vector3 movement = Vector3.zero;
            Vector3 forward = stateMachine.mainCamera.transform.forward;
            Vector3 right = stateMachine.mainCamera.transform.right;
            forward.y = 0f;
            right.y = 0f;


            movement += right * stateMachine.inputReader.movementValue.x;
            movement += forward * stateMachine.inputReader.movementValue.y;
            return movement;
        }
    }
}
