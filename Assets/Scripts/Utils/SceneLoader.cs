using System;
using System.Collections;
using System.Threading;
using MyExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : SingletonDontDestroy<SceneLoader>
{
    [SerializeField] private CanvasGroup cg;
    [Space]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text progressText;

    private CancellationTokenSource ctx;



    public async void LoadScene(string sceneName, float duration = .5f)
    {
        ctx?.Cancel();
        ctx = new CancellationTokenSource();
        try
        {
            cg.alpha = 0f;
            slider.value = 0f;
            progressText.text = 0 + "%";
            cg.gameObject.SetActive(true);
            await cg.LerpAlpha(1f, duration, ctx.Token);   //80 es el minimo
        }
        catch (OperationCanceledException)
        {
            // La operación fue cancelada, no hacemos nada
            return;
        }
    }

    public async void LoadScene(int sceneIndex, float duration = .5f)
    {
        ctx?.Cancel();
        ctx = new CancellationTokenSource();
        try
        {
            cg.alpha = 0f;
            slider.value = 0f;
            progressText.text = 0 + "%";
            cg.gameObject.SetActive(true);
            await cg.LerpAlpha(1f, duration, ctx.Token);   //80 es el minimo
            StartCoroutine(LoadLevelAsync(sceneIndex));
        }
        catch (OperationCanceledException)
        {
            // La operación fue cancelada, no hacemos nada
            return;
        }
    }

    private IEnumerator LoadLevelAsync(int sceneIndex, float duration = .5f)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            slider.value = progress;
            progressText.text = progress * 100f + "%";
            yield return null;
        }

        ctx?.Cancel();
        ctx = new CancellationTokenSource();
        try
        {
            _ = cg.LerpAlpha(0f, duration, ctx.Token);   //80 es el minimo
        }
        catch (OperationCanceledException)
        {
            // La operación fue cancelada, no hacemos nada
            yield break;
        }
    }
}
