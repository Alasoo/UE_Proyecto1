using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


namespace MyExtensions
{
    public static class Extensions
    {
        public static async void WrapErrors(this UniTask task)
        {
            await task;
        }


        public static IEnumerator AsCoroutine(this System.Threading.Tasks.Task task)
        {
            while (!task.IsCompleted)
            {
                if (task.IsFaulted)
                    throw task.Exception;

                yield return null;
            }

            if (task.IsFaulted)
                throw task.Exception;
        }

        public static IEnumerator AsCoroutine(this UniTask task)
        {
            // Obtenemos el awaiter para comprobar el estado de la tarea
            var awaiter = task.GetAwaiter();

            while (!awaiter.IsCompleted)
            {
                // Esperamos al siguiente frame de Unity
                yield return null;
            }

            awaiter.GetResult();
        }



        public static async UniTask LerpScale(this Transform transform, Vector3 targetScale, float duration, CancellationToken token)
        {
            var time = 0f;
            var startScale = transform.localScale;
            while (time < duration && !token.IsCancellationRequested)
            {
                transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
                time += Time.unscaledDeltaTime;
                await UniTask.Yield();
            }


            if (!token.IsCancellationRequested)
            {
                transform.localScale = targetScale;
            }
            else
                throw new TaskCanceledException();
        }




        public static async UniTask LerpAlpha(this CanvasGroup cg, float target, float duration, CancellationToken token)
        {
            var time = 0f;
            var startValue = cg.alpha;
            while (time < duration && !token.IsCancellationRequested)
            {
                cg.alpha = Mathf.Lerp(startValue, target, time / duration);
                time += Time.unscaledDeltaTime;
                await UniTask.Yield();
            }


            if (!token.IsCancellationRequested)
            {
                cg.alpha = target;
            }
            else
                throw new TaskCanceledException();
        }


        public static async UniTask LerpAlpha(this Image img, float target, float duration, CancellationToken token)
        {
            var time = 0f;
            var startValue = img.color.a;
            while (time < duration && !token.IsCancellationRequested)
            {
                var color = img.color;
                color.a = Mathf.Lerp(startValue, target, time / duration);
                img.color = color;
                time += Time.unscaledDeltaTime;
                await UniTask.Yield();
            }


            if (!token.IsCancellationRequested)
            {
                Color color = img.color;
                color.a = target;
                img.color = color;
            }
            else
                throw new TaskCanceledException();
        }


        public static async UniTask LerpAlpha(this SpriteRenderer img, float target, float duration, CancellationToken token)
        {
            var time = 0f;
            var startValue = img.color.a;
            while (time < duration && !token.IsCancellationRequested)
            {
                var color = img.color;
                color.a = Mathf.Lerp(startValue, target, time / duration);
                img.color = color;
                time += Time.unscaledDeltaTime;
                await UniTask.Yield();
            }


            if (!token.IsCancellationRequested)
            {
                Color color = img.color;
                color.a = target;
                img.color = color;
            }
            else
                throw new TaskCanceledException();
        }




        public static async UniTask LerpRectMovementX(this RectTransform rt, float target, float duration, CancellationToken token)
        {
            var time = 0f;
            var startValue = rt.anchoredPosition.x;
            while (time < duration && !token.IsCancellationRequested)
            {
                rt.anchoredPosition = Vector2.Lerp(new Vector2(startValue, rt.anchoredPosition.y), new Vector2(target, rt.anchoredPosition.y), time / duration);
                time += Time.unscaledDeltaTime;
                await UniTask.Yield();
            }


            if (!token.IsCancellationRequested)
            {
                rt.anchoredPosition = new Vector2(target, rt.anchoredPosition.y);
            }
            else
                throw new TaskCanceledException();
        }


        public static async UniTask LerpAudio(this AudioMixer audio, string audioLabel, float target, float duration, CancellationToken token)
        {
            var time = 0f;
            audio.GetFloat(audioLabel, out float startValue);
            while (time < duration && !token.IsCancellationRequested)
            {
                audio.SetFloat(audioLabel, Mathf.Lerp(startValue, target, time / duration));
                time += Time.unscaledDeltaTime;
                await UniTask.Yield();
            }

            if (!token.IsCancellationRequested)
            {
                audio.SetFloat(audioLabel, target);
            }
            else
                throw new TaskCanceledException();
        }

        public static void ClearCts(this CancellationTokenSource cts)
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }


        public static T RandomElement<T>(this List<T> list)
        {
            if (list.Count == 0)
            {
                return default;
            }
            return list[UnityEngine.Random.Range(0, list.Count)];
        }


        public static void ClearCts(ref CancellationTokenSource cts)
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = null;
        }



    }

}





/*

        protected CancellationTokenSource ctx;


            try
            {
                ctx?.Cancel();
                ctx = new CancellationTokenSource();
                //_ = rt.LerpRTMovement(endPos, .3f, ctx.Token);
            }
            catch (OperationCanceledException)
            {


            }
            catch (Exception e)
            {


            }

*/