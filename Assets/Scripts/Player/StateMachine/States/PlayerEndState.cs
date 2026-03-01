using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller.Player
{
    public class PlayerEndState : PlayerBaseState
    {
        //sabiendo el gameEnd puedo hacer animacion de celebración
        public PlayerEndState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter() { }
        public override void Tick(float deltaTime) { }
        public override void FixedTick(float deltaTime) { }
        public override void Exit() { }
        public override void OnDestroy() { }

    }

}