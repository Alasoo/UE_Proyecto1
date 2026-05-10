using System.Collections;
using System.Collections.Generic;
using HealthSystem;
using UnityEngine;


namespace Controller.Player
{
    [RequireComponent(typeof(InputReader))]
    public class PlayerStateMachine : StateMachine
    {
        public static PlayerStateMachine Instance;

        [field: Header("COMPONENTS")]
        [field: SerializeField] public InputReader inputReader { get; private set; }
        [field: SerializeField] public Rigidbody2D rb { get; private set; }
        [field: SerializeField] public Camera mainCamera { get; private set; }
        [field: SerializeField] public SpriteRenderer spriteRenderer { get; private set; }
        [field: SerializeField] public Health health { get; private set; }


        [field: Header("SPEEDS")]
        public float moveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float sprintSpeed = 5.335f;




        [field: SerializeField] public float turnSmoothTime = .1f;

        public bool canMove { get; private set; } = true;


        
        public PlayerStats playerStats { get; private set; } = new();


        private void Awake()
        {
            Instance = this;
            health.Init(200, spriteRenderer.material);
        }

        public void SetCanMove(bool value)
        {
            if (canMove == value) return;
            canMove = value;
            if (!canMove)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }




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



