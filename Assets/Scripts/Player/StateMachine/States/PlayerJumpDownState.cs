using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller.Player
{
    public class PlayerJumpDownState : PlayerBaseState
    {
        private readonly int jumpHash = Animator.StringToHash("jumpDown");
        private readonly int emptyJumpHash = Animator.StringToHash("emptyJump");


        public PlayerJumpDownState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        //Time to blend anims
        private const float CrossFadeDuration = .0f;

        public override void Enter()
        {
            Debug.Log($"Enter jumpDown");
        }


        public override void Tick(float deltaTime)
        {
            /*
            //Move(deltaTime);
            if (GetNormalizedTime(stateMachine.animator, "jumpDown", 1) >= 1f)
            {
                Debug.Log($"go free");
                stateMachine.SwitchState(new PlayerFreeLocomotionState(stateMachine));
            }
            */
        }



        public override void Exit()
        {

        }
        public override void OnDestroy() { }


  


    }

}