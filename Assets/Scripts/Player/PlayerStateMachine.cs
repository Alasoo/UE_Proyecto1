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
        [field: SerializeField] public PlayerUI playerUI { get; private set; }
        [field: SerializeField] public PlayerAutoAttack playerAutoAttack { get; private set; }
        [field: SerializeField] public CircleCollider2D circleCollider { get; private set; }






        [field: SerializeField] public float turnSmoothTime = .1f;

        public bool canMove { get; private set; } = true;



        public PlayerStats playerStats { get; private set; } = new();


        private void Awake()
        {
            Instance = this;
            playerUI.Init(playerStats, spriteRenderer.material);
            playerAutoAttack.Init(playerStats);
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



