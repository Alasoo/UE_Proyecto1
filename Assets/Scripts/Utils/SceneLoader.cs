using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
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

    [Header("SCENES")]
    [SerializeField] private int menuIndex;
    [SerializeField] private int gameIndex;

    private float duration = .5f;


    private CancellationTokenSource ctsFade, ctsLoad;

    private void OnDestroy()
    {
        ctsFade?.ClearCts();
        ctsFade = null;
        ctsLoad?.ClearCts();
        ctsLoad = null;
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

        await FadeOn();

        AsyncOperation op = SceneManager.LoadSceneAsync(menuIndex);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            slider.value = progress;
            progressText.text = (progress * 100f).ToString("F0") + "%";                  //la carga sera el 20%, la creación del mapa en el awake sera el otro 80%
            await UniTask.Yield(cancellationToken: ctsLoad.Token);
        }

        slider.value = 1f;
        progressText.text = "100%";

        _ = FadeOff();
    }

    public async UniTask LoadGame()
    {
        ctsLoad?.Cancel();
        ctsLoad = new();

        Debug.Log($"1 -- {Time.time}");
        await FadeOn();
        Debug.Log($"2 -- {Time.time}");

        AsyncOperation op = SceneManager.LoadSceneAsync(gameIndex);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f) { await UniTask.Yield(cancellationToken: ctsLoad.Token); }
        op.allowSceneActivation = true;

        Debug.Log($"3 -- {Time.time}");
        await UniTask.WaitUntil(() => MapCreator.Instance != null, cancellationToken: ctsLoad.Token);
        Debug.Log($"4 -- {Time.time}");
        MapCreator.Instance.OnProgress += OnProgress;
        await MapCreator.Instance.Init();
        MapCreator.Instance.OnProgress -= OnProgress;

        Debug.Log($"5 -- {Time.time}");
        await UniTask.WaitForSeconds(.5f);      //pequeño delay para que se vea el 100%
        _ = FadeOff();
        Debug.Log($"6 -- {Time.time}");
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

        }
        finally
        {
            ctsFade?.ClearCts();
            ctsFade = null;
        }
    }

    private async UniTask FadeOff()
    {
        ctsFade?.Cancel();
        ctsFade = new CancellationTokenSource();
        try
        {
            Debug.Log($"Apagando Canvas");
            await cg.LerpAlpha(0f, duration, ctsFade.Token);
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }
        catch (OperationCanceledException)
        {
            Debug.LogError($"Cancelado");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e}");
            return;
        }
        finally
        {
            ctsFade?.ClearCts();
            ctsFade = null;
        }
    }



}
