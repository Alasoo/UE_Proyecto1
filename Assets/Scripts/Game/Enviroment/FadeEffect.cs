using System.Threading;
using UnityEngine;
using MyExtensions;
using Cysharp.Threading.Tasks;
using System;
using Controller.Player;


namespace GameSystem.Effects
{
    public class FadeEffect : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float fadeDuration = .35f;


        private CancellationTokenSource ctsEffect;


        private void OnDestroy()
        {
            ctsEffect?.ClearCts();
            ctsEffect = null;
        }


        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject != PlayerStateMachine.Instance.gameObject) return;
            _ = Fade(true);
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject != PlayerStateMachine.Instance.gameObject) return;
            _ = Fade(false);
        }


        private async UniTask Fade(bool enter)
        {
            ctsEffect?.Cancel();
            ctsEffect = new();
            try
            {
                if (enter)
                {
                    await spriteRenderer.LerpAlpha(0.5f, fadeDuration, ctsEffect.Token);
                }
                else
                {
                    await spriteRenderer.LerpAlpha(1f, fadeDuration, ctsEffect.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"fadeCanceled");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error: {e}");
            }
            finally
            {
                ctsEffect?.ClearCts();
                ctsEffect = null;
            }
        }
    }
}
