using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using MyExtensions;
using UnityEngine;


namespace MyUI.Panels
{
    [RequireComponent(typeof(CanvasGroup))]

    public class MenuState : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup panel;

        protected CancellationTokenSource ctx1;
        protected Action actionClicked;

#if UNITY_EDITOR
        void OnValidate()
        {
            if (panel == null)
                panel = GetComponent<CanvasGroup>();
        }
#endif


        public virtual void Init() { }


        void OnDestroy()
        {
            ctx1?.Cancel();
        }

        public virtual void Enter()
        {
            FadeIn();
        }

        public virtual void Exit()
        {
            FadeOut();
        }


        protected void FadeIn()
        {
            panel.interactable = true;

            //si estaba apagandose lo cancelo
            ctx1?.Cancel();
            ctx1 = new CancellationTokenSource();
            panel.alpha = 0;
            panel.gameObject.SetActive(true);
            try
            {
                _ = panel.LerpAlpha(1, .2f, ctx1.Token);
            }
            catch (OperationCanceledException)
            {
                // La operación fue cancelada, no hacemos nada
                return;
            }

            //panel.interactable = true;
            //panel.alpha = 1;
            //panel.gameObject.SetActive(true);
        }


        protected async void FadeOut()
        {

            ctx1?.Cancel();
            ctx1 = new CancellationTokenSource();
            panel.interactable = false;
            try
            {
                await panel.LerpAlpha(0, .2f, ctx1.Token);
                if (!ctx1.IsCancellationRequested)
                {
                    panel.gameObject.SetActive(false);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }

            //panel.interactable = false;
            //panel.alpha = 0;
            //panel.gameObject.SetActive(false);
        }


        public virtual void ForceClose()
        {
            panel.gameObject.SetActive(false);
            panel.interactable = false;
            panel.alpha = 0;
        }

        public virtual void ForceOpen()
        {
            panel.gameObject.SetActive(true);
            panel.interactable = false;
            panel.alpha = 0;
        }

        protected virtual void OnClickBack()
        {
            MenuStateMachine.Instance.GoBack();
        }



    }
}
