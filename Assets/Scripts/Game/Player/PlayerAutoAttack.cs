using Controller.Player;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MyExtensions;
using System.Threading;
using System;
using BulletSystem;
using System.Collections.Generic;


public class PlayerAutoAttack : MonoBehaviour
{

    private CancellationTokenSource ctsAutoAttack;
    private PlayerStats playerStats;

    [SerializeField] private BulletScriptable bulletScriptable;


    private List<GameObject> enemiesOnRange = new();

    private bool attack = false;


    public async void Init(PlayerStats playerStats)
    {
        this.playerStats = playerStats;

        playerStats.OnDie += OnDie;
        var bulletData = bulletScriptable.TakeBulletPrefab();
        //_ = BulletPool.Instance.CreateBullet(bulletData.bullet, bulletData.count, bulletScriptable);

        ctsAutoAttack?.Cancel();
        ctsAutoAttack = new();
        attack = true;
        _ = AutoAttackTask();
    }


    void OnDestroy()
    {
        attack = false;
        Extensions.ClearCts(ref ctsAutoAttack);
        playerStats.OnDie -= OnDie;
    }

    private void OnDie()
    {
        //attack = false;
        //Extensions.ClearCts(ref ctsAutoAttack);
    }

    private async UniTask AutoAttackTask()
    {
        try
        {
            while (attack)
            {
                ctsAutoAttack.Token.ThrowIfCancellationRequested();
                await UniTask.WaitUntil(() => enemiesOnRange.Count > 0);
                var bulletData = bulletScriptable.TakeBulletPrefab();
                Transform enemy = await TakeCloseEnemy();
                Vector3 directionToEnemy = (enemy.position - transform.position).normalized;
                Vector3 spawnPosition = transform.position + (directionToEnemy * 0.5f);
                BulletPool.Instance.Get(bulletData.bullet, directionToEnemy, spawnPosition, bulletScriptable);

                await UniTask.WaitForSeconds(playerStats.GetSpeedAttack, cancellationToken: ctsAutoAttack.Token);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log($"Cancelado AUTOATTACK");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error: {e}");
        }

    }

    private async UniTask<Transform> TakeCloseEnemy()
    {
        List<GameObject> cloneList = new(enemiesOnRange);   //hago una clonacion por si se modificara mientras la leo
        if (cloneList.Count == 0) return null;

        Transform closeEnemy = null;
        float currentDistance = 1000f;


        for (int i = 0; i < cloneList.Count; i++)
        {
            Vector3 direction = transform.position - cloneList[i].transform.position;
            float distance = direction.sqrMagnitude;

            if (distance < currentDistance)
            {
                currentDistance = distance;
                closeEnemy = cloneList[i].transform;
            }
            await UniTask.Yield(ctsAutoAttack.Token);
        }

        return closeEnemy;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        enemiesOnRange.Add(collision.gameObject);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        enemiesOnRange.Remove(collision.gameObject);
    }



}
