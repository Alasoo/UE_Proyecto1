using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller
{
    public class State
    {
        public virtual void Enter() { }

        public virtual void Tick(float deltaTime) { }
        public virtual void FixedTick(float fixedDeltaTime) { }

        public virtual void Exit() { }
        public virtual void OnDestroy() { }

        protected float GetNormalizedTime(Animator animator, string tag)
        {
            AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

            if (animator.IsInTransition(0) && nextInfo.IsTag(tag))
            {
                return nextInfo.normalizedTime;
            }
            else if (!animator.IsInTransition(0) && currentInfo.IsTag(tag))
            {
                return currentInfo.normalizedTime;
            }
            else
            {
                return 0f;
            }
        }
        protected float GetNormalizedTime(Animator animator, string tag, int layer)
        {
            AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(layer);
            AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(layer);

            if (animator.IsInTransition(layer) && nextInfo.IsTag(tag))
            {
                return nextInfo.normalizedTime;
            }
            else if (!animator.IsInTransition(layer) && currentInfo.IsTag(tag))
            {
                return currentInfo.normalizedTime;
            }
            else
            {
                return 0f;
            }
        }

        protected float GetNormalizedTime(Animator animator)
        {
            AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

            if (animator.IsInTransition(0))
            {
                return nextInfo.normalizedTime;
            }
            else if (!animator.IsInTransition(0))
            {
                return currentInfo.normalizedTime;
            }
            else
            {
                return 0f;
            }
        }
    }
}
