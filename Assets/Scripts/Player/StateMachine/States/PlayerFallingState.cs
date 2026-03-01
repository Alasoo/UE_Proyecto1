using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller.Player
{
    public class PlayerFallingState : PlayerBaseState
    {
        private readonly int fallHash = Animator.StringToHash("falling");

        public PlayerFallingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        //Time to blend anims
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