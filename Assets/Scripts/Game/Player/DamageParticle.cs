using System;
using System.Collections.Generic;
using System.Threading;
using Controller.Player;
using Cysharp.Threading.Tasks;
using MyExtensions;
using TMPro;
using UnityEngine;

public class DamageParticle : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [Space]
    [SerializeField] private TMP_Text damageTextPrefab;


    private Stack<TMP_Text> damageTextStack = new();
    private List<CancellationTokenSource> ctsList = new();


    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            TMP_Text clone = Instantiate(damageTextPrefab, canvas.transform);
            damageTextStack.Push(clone);
        }

        PlayerStateMachine.Instance.playerStats.OnTakeDamage += OnTakeDamage;
    }

    void OnDestroy()
    {
        for (int i = 0; i < ctsList.Count; i++)
        {
            ctsList[i]?.Cancel();
            ctsList[i]?.Dispose();
        }
        ctsList.Clear();
        if (PlayerStateMachine.Instance == null) return;
        PlayerStateMachine.Instance.playerStats.OnTakeDamage -= OnTakeDamage;
    }

    private void OnTakeDamage(int damage)
    {
        TMP_Text texto = null;
        if (damageTextStack.Count > 0)
        {
            texto = damageTextStack.Pop();
            texto.text = damage.ToString();
        }
        else
        {
            texto = Instantiate(damageTextPrefab, canvas.transform);
        }

        _ = Effect(texto);
    }

    private async UniTask Effect(TMP_Text texto)
    {
        texto.transform.position = PlayerStateMachine.Instance.transform.position;
        texto.alpha = 1;
        texto.gameObject.SetActive(true);

        CancellationTokenSource cts = new();
        ctsList.Add(cts);

        try
        {
            var movementTask = Movement(texto, cts.Token);
            var fadeTask = Fade(texto, cts.Token);
            await UniTask.WhenAll(movementTask, fadeTask);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        damageTextStack.Push(texto);
        texto.gameObject.SetActive(false);
        if (ctsList.Contains(cts))
        {
            ctsList.Remove(cts);
            Extensions.ClearCts(ref cts);
        }
    }

    private async UniTask Movement(TMP_Text texto, CancellationToken token)
    {
        try
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            await texto.LerpMovement(pos, 1f, token);
        }
        catch (OperationCanceledException) { throw; }
    }

    private async UniTask Fade(TMP_Text texto, CancellationToken token)
    {
        try
        {
            await texto.LerpAlpha(0, 1f, token);
        }
        catch (OperationCanceledException) { throw; }
    }

}