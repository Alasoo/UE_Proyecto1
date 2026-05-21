using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller
{
    public class StateMachine : MonoBehaviour
    {
        private State currentState;

        public void SwitchState(State newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }

        private void Update()
        {
            currentState?.Tick(Time.deltaTime);
        }

        private void LateUpdate()
        {
            currentState?.LateTick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            currentState?.FixedTick(Time.fixedDeltaTime);
        }

        protected virtual void OnDestroy()
        {
            currentState?.OnDestroy();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            currentState?.OnTriggerEnter2D(collision);
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            currentState?.OnTriggerExit2D(collision);
        }

        public void PlayerOnRange(bool inRange)
        {
            currentState?.PlayerOnRange(inRange);
        }



    }
}
