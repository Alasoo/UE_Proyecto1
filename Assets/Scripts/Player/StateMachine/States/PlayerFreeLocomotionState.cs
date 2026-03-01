using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller.Player
{

    public class PlayerFreeLocomotionState : PlayerBaseState
    {
        public PlayerFreeLocomotionState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private const float crossFadeDuration = .1f;

        private Vector2 direction;

        public override void Enter()
        {
        }


        public override void Tick(float deltaTime)
        {
            if (stateMachine.inputReader.movementValue.magnitude <= .1f)
            {
                direction = Vector2.zero;
                return;
            }
            direction = Direction();
            Rotate(direction);
        }

        public override void FixedTick(float fixedDeltaTime)
        {
            Move(fixedDeltaTime, direction);
        }


        public override void Exit()
        {
        }


    }
}