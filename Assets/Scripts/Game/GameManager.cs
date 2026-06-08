using System;
using System.Collections.Generic;
using System.Threading;
using Controller.Enemy;
using Controller.Player;
using Cysharp.Threading.Tasks;
using EnemySystem;
using GameSystem;
using MyExtensions;
using SaveSystem;
using TMPro;
using UnityEngine;


public class GameManager : Singleton<GameManager>
{
    [Header("GAME TIME")]
    [SerializeField] private TMP_Text gameTimeText;


    private float gameTime = 0f;
    private GameInfo currentGame = null;

    private bool startGame = false;

    private List<CancellationTokenSource> spawnCts = new();


    void Start()
    {
        PlayerStateMachine.Instance.playerStats.OnDie += OnPlayerDie;
        currentGame = new();
        gameTimeText.text = "00:00:00";

        EnemyCreator.Instance.OnDieWave += OnDieWave;
    }

    void OnDestroy()
    {
        foreach (var cts in spawnCts)
            cts?.ClearCts();
        spawnCts.Clear();

        if (PlayerStateMachine.Instance != null && PlayerStateMachine.Instance.playerStats != null)
            PlayerStateMachine.Instance.playerStats.OnDie -= OnPlayerDie;

        if (EnemyCreator.Instance != null)
            EnemyCreator.Instance.OnDieWave -= OnDieWave;
    }

    private void OnPlayerDie()
    {
        currentGame.totalSeconds = (int)gameTime;
        //podria guardar cuantos magos he matado, curaciones etc...

        var gameWrap = SaveLoadManager<GameWrap>.LoadData(GameWrap.GAME_KEY);
        if (gameWrap.success)
        {
            gameWrap.data.games.Add(currentGame);
            SaveLoadManager<GameWrap>.SaveData(GameWrap.GAME_KEY, gameWrap.data);
            return;
        }

        GameWrap newGameWrap = new();
        newGameWrap.games.Add(currentGame);
        SaveLoadManager<GameWrap>.SaveData(GameWrap.GAME_KEY, newGameWrap);
    }

    public void StartGame()
    {
        gameTime = 0f;
        startGame = true;

        foreach (var wave in EnemyCreator.Instance.waves)
        {
            _ = SpawnEnemies(wave.Key, wave.Value);
        }
    }


    private void Update()
    {
        if (!startGame) return;
        gameTime += Time.deltaTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(gameTime);
        gameTimeText.text = $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes}:{timeSpan.Seconds}";
    }


    private async UniTask SpawnEnemies(WaveData waveData, List<EnemyStateMachine> enemies)
    {
        CancellationTokenSource cts = new();
        spawnCts.Add(cts);
        try
        {
            await UniTask.WaitForSeconds(waveData.secondToSpawn, cancellationToken: cts.Token);
            cts.Token.ThrowIfCancellationRequested();
            await EnemyCreator.Instance.SpawnWaveAsync(waveData, enemies);
        }
        catch (OperationCanceledException) { }
        finally
        {
            if (spawnCts.Contains(cts))
            {
                spawnCts.Remove(cts);
                cts?.ClearCts();
            }
        }
    }

    private void OnDieWave(WaveData waveData)
    {
        List<EnemyStateMachine> enemiesToSpawn = EnemyCreator.Instance.waves[waveData];
        _ = SpawnEnemies(waveData, enemiesToSpawn);
    }
}
