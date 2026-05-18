using System;
using Controller.Player;
using GameSystem;
using SaveSystem;
using TMPro;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("GAME TIME")]
    [SerializeField] private TMP_Text gameTimeText;


    private float gameTime = 0f;
    private GameInfo currentGame = null;

    public event Action<int> OnUpdateTime;
    private bool startGame = false;


    void Start()
    {
        PlayerStateMachine.Instance.playerStats.OnDie += OnPlayerDie;
        currentGame = new();
        gameTimeText.text = "00:00:00";
    }

    void OnDestroy()
    {
        if (PlayerStateMachine.Instance != null && PlayerStateMachine.Instance.playerStats != null)
            PlayerStateMachine.Instance.playerStats.OnDie -= OnPlayerDie;
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
    }


    private void Update()
    {
        if (!startGame) return;
        int lastSec = (int)gameTime;
        gameTime += Time.deltaTime;
        int newSecond = (int)gameTime;
        if (lastSec != newSecond)
            OnUpdateTime?.Invoke(newSecond);

        TimeSpan timeSpan = TimeSpan.FromSeconds(gameTime);
        gameTimeText.text = $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes}:{timeSpan.Seconds}";
    }
}
