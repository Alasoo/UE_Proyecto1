using Controller.Player;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MyExtensions;
using System.Threading;
using System;


public class PlayerAutoAttack : MonoBehaviour
{

    private CancellationTokenSource ctsAutoAttack;
    private PlayerStats playerStats;


    public void Init(PlayerStats playerStats)
    {
        this.playerStats = playerStats;

        playerStats.OnDie += OnDie;

        ctsAutoAttack?.Cancel();
        ctsAutoAttack = new();
        _ = AutoAttackTask();
    }


    void OnDestroy()
    {
        Extensions.ClearCts(ref ctsAutoAttack);
        playerStats.OnDie -= OnDie;
    }

    private void OnDie()
    {
        Extensions.ClearCts(ref ctsAutoAttack);
    }

    private async UniTask AutoAttackTask()
    {
        try
        {
            while (true)
            {
                ctsAutoAttack.Token.ThrowIfCancellationRequested();
                Debug.Log($"Attack");

                await UniTask.WaitForSeconds(playerStats.GetSpeedAttack, cancellationToken: ctsAutoAttack.Token);
            }
        }
        catch (OperationCanceledException) { }

    }




}
