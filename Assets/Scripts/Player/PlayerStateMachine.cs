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
        //[field: SerializeField] public Animator animator { get; private set; }
        [field: SerializeField] public Rigidbody rb { get; private set; }
        [field: SerializeField] public Camera mainCamera { get; private set; }
        [field: SerializeField] public Transform groundCheck { get; private set; }


        [field: Header("SPEEDS")]
        public float moveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float sprintSpeed = 5.335f;
        public bool grounded = true;

        [field: SerializeField] public float groundedOffset { get; private set; } = -0.14f;
        [field: SerializeField] public float groundedRadius { get; private set; } = 0.28f;
        [field: SerializeField] public LayerMask groundLayers { get; private set; }


        public float jumpForce = 5f;
        public bool jumpRequested { get; set; }
        public bool isLeavingGround { get; set; }
        public float fallMultiplier = 2.5f;

        void Awake()
        {

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


#if UNITY_EDITOR
        // Dibujar el gizmo del GroundCheck en la escena
        private void OnDrawGizmos()
        {

            if (groundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundedRadius * 2);
            }

        }
#endif
    }
}



