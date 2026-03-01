using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller.Player
{
    [RequireComponent(typeof(InputReader))]
    public class PlayerStateMachine : StateMachine
    {
        public static PlayerStateMachine player;

        [field: Header("COMPONENTS")]
        [field: SerializeField] public InputReader inputReader { get; private set; }
        [field: SerializeField] public Rigidbody2D rb { get; private set; }
        [field: SerializeField] public Camera mainCamera { get; private set; }


        [field: Header("SPEEDS")]
        public float moveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float sprintSpeed = 5.335f;


        [field: SerializeField] public float turnSmoothTime = .1f;

        private void Start()
        {
            SwitchState(new PlayerFreeLocomotionState(this));
        }



        void OnEnable()
        {

        }


        void OnDisable()
        {

        }

        private void OnDie()
        {
            SwitchState(new PlayerDeadState(this));
        }

    }
}



