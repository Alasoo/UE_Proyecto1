using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using MyExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameSystem
{
    public class SceneLoader : SingletonDontDestroy<SceneLoader>
    {
        [SerializeField] private CanvasGroup cg;
        [Space]
        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text progressText;

        [Header("SCENES")]
        [SerializeField] private int menuIndex;
        [SerializeField] private int gameIndex;

        private float duration = .5f;


        private CancellationTokenSource ctsFade, ctsLoad;

        private void OnDestroy()
        {
            Extensions.ClearCts(ref ctsFade);
            Extensions.ClearCts(ref ctsLoad);
        }


        private void Start()
        {
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        public async UniTask LoadMenu()
        {
            ctsLoad?.Cancel();
            ctsLoad = new();

            try
            {
                await FadeOn();

                AsyncOperation op = SceneManager.LoadSceneAsync(menuIndex);
                op.allowSceneActivation = false;

                while (op.progress < 0.9f)
                {
                    float progress = Mathf.Clamp01(op.progress / 0.9f);
                    slider.value = progress;
                    progressText.text = (progress * 100f).ToString("F0") + "%";
                    await UniTask.Yield(cancellationToken: ctsLoad.Token);
                }

                slider.value = 1f;
                progressText.text = "100%";

                op.allowSceneActivation = true;

                await UniTask.Yield(cancellationToken: ctsLoad.Token);

                await FadeOff();
            }
            catch (OperationCanceledException) { }
            finally
            {
                Extensions.ClearCts(ref ctsFade);
                Extensions.ClearCts(ref ctsLoad);
            }
        }

        public async UniTask LoadGame()
        {
            ctsLoad?.Cancel();
            ctsLoad = new CancellationTokenSource();

            // 1. Guardamos el token en una variable struct. 
            // Aunque ctsLoad se vuelva null en OnDestroy, esta variable sobrevivirá.
            CancellationToken token = ctsLoad.Token;

            try
            {
                await FadeOn();

                AsyncOperation op = SceneManager.LoadSceneAsync(gameIndex);
                op.allowSceneActivation = false;

                // Usamos la variable 'token'
                while (op.progress < 0.9f) { await UniTask.Yield(cancellationToken: token); }
                op.allowSceneActivation = true;

                await UniTask.WaitUntil(() => MapCreator.Instance != null, cancellationToken: token);

                MapCreator.Instance.OnProgress += OnProgress;
                await MapCreator.Instance.Init();

                // Usamos la variable 'token'
                token.ThrowIfCancellationRequested();

                MapCreator.Instance.OnProgress -= OnProgress;

                // 2. Le pasamos el token al delay
                await UniTask.WaitForSeconds(.5f, cancellationToken: token);

                await FadeOff();
            }
            catch (OperationCanceledException)
            {
                // Cancelación controlada al quitar el Play
                Debug.Log("LoadGame cancelado correctamente.");
            }
            catch (Exception e)
            {
                // Si ocurre CUALQUIER otro error, queremos verlo en consola
                Debug.LogError($"Error crítico en LoadGame: {e}");
            }
            finally
            {
                // En caso de que se haya modificado OnProgress y haya fallado a medias
                if (MapCreator.Instance != null)
                {
                    MapCreator.Instance.OnProgress -= OnProgress;
                }

                Extensions.ClearCts(ref ctsFade);
                Extensions.ClearCts(ref ctsLoad);
            }
        }
        
        private void OnProgress(float progress)
        {
            slider.value = progress;
            progressText.text = (progress * 100f).ToString("F0") + "%";
        }


        private async UniTask FadeOn()
        {
            ctsFade?.Cancel();
            ctsFade = new CancellationTokenSource();
            try
            {
                slider.value = 0f;
                progressText.text = (0 * 100f).ToString("F0") + "%";
                cg.alpha = 0f;
                cg.interactable = true;
                cg.blocksRaycasts = true;
                await cg.LerpAlpha(1f, duration, ctsFade.Token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            finally
            {
                Extensions.ClearCts(ref ctsFade);
            }
        }

        private async UniTask FadeOff()
        {
            ctsFade?.Cancel();
            ctsFade = new CancellationTokenSource();
            try
            {
                await cg.LerpAlpha(0f, duration, ctsFade.Token);
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
            catch (OperationCanceledException)
            {
                Debug.LogError($"Cancelado");
                throw;
            }
            finally
            {
                Extensions.ClearCts(ref ctsFade);
            }
        }


    }
}
