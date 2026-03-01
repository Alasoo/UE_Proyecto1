using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller.Player
{
    public class PlayerDeadState : PlayerBaseState
    {

        public PlayerDeadState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        private float timeToPopup = 2f;


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