using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller.Player
{
    public class InputReader : MonoBehaviour, Controls.IPlayerActions
    {
        public Vector2 movementValue { get; private set; }
        public bool aiming { get; private set; }
        public bool sprint { get; private set; } = false;
        public bool isAttacking { get; set; }

        public event Action InteractEvent;
        public event Action TargetEvent;
        public event Action OnCancelEvent;


        private Controls controls;


        private void Awake()
        {
            controls = new Controls(); // Crea una nueva instancia (no usa el singleton)
            controls.Player.SetCallbacks(this);
        }

        private void OnEnable()
        {
            controls.Player.Enable(); // Activar el action map Player
        }

        private void OnDisable()
        {
            controls.Player.Disable(); // Desactivar el action map Player
        }






        public void OnMove(InputAction.CallbackContext context)
        {
            movementValue = context.ReadValue<Vector2>();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            InteractEvent?.Invoke();
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            OnCancelEvent?.Invoke();
        }

        public void OnLook(InputAction.CallbackContext context)
        {

        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
                isAttacking = true;
            else if (context.canceled)
                isAttacking = false;
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {

        }


        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started)
                sprint = true;
            else if (context.canceled)
                sprint = false;
        }

        public void OnZoom(InputAction.CallbackContext context)
        {

        }

        public void OnTarget(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            TargetEvent?.Invoke();
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
        }

        public void OnNext(InputAction.CallbackContext context)
        {
        }
    }
}









/*
 public void OnPause(InputAction.CallbackContext context)
 {
     if (!context.started) return;
     PauseEffect();
 }

 public void PauseEffect()
 {
     PauseEvent?.Invoke();
     if (Settings.inUI)
     {
         playerInput.SwitchCurrentActionMap("UI");
     }
     else
     {
         playerInput.SwitchCurrentActionMap("Player");
     }
 }
 */



