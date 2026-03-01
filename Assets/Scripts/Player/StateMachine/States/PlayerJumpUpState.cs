using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller.Player
{

    public class PlayerJumpUpState : PlayerBaseState
    {
        private readonly int jumpHash = Animator.StringToHash("jumpUp");

        public PlayerJumpUpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private const float CrossFadeDuration = .2f;

        public override void Enter()
        {

        }


        public override void Tick(float deltaTime)
        {

        }




        public override void Exit()
        {

        }





    }

}