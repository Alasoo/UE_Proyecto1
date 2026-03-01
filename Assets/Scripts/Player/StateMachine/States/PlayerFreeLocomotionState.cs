using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller.Player
{

    public class PlayerFreeLocomotionState : PlayerBaseState
    {
        public PlayerFreeLocomotionState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private const float crossFadeDuration = .1f;

        private Vector3 direction;

        public override void Enter()
        {
            stateMachine.inputReader.OnJumpEvent += OnJump;
        }


        public override void Tick(float deltaTime)
        {
            GroundedCheck();
            direction = Direction();
        }

        public override void FixedTick(float fixedDeltaTime)
        {
            Move(fixedDeltaTime, direction);
            JumpEffect();
            FallingEffect();
        }


        public override void Exit()
        {
            stateMachine.inputReader.OnJumpEvent -= OnJump;
        }

        private void OnJump()
        {
            Jump();
        }

    }
}