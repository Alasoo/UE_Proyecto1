using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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
            Extensions.ClearCts(ref ctx1);
        }

        public virtual void Enter()
        {
            FadeIn().Forget();
        }

        public virtual void Exit()
        {
            FadeOut().Forget();
        }


        protected async UniTaskVoid FadeIn()
        {
            panel.interactable = true;

            //si estaba apagandose lo cancelo
            ctx1?.Cancel();
            ctx1 = new CancellationTokenSource();
            panel.alpha = 0;
            panel.gameObject.SetActive(true);
            try
            {
                await panel.LerpAlpha(1, .2f, ctx1.Token);
            }
            catch (OperationCanceledException)
            {
                // La operación fue cancelada, no hacemos nada
                return;
            }
            finally
            {
                Extensions.ClearCts(ref ctx1);
            }

            //panel.interactable = true;
            //panel.alpha = 1;
            //panel.gameObject.SetActive(true);
        }


        protected async UniTaskVoid FadeOut()
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
            finally
            {
                Extensions.ClearCts(ref ctx1);
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
