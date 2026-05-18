using System;
using GameSystem;
using TMPro;
using UnityEngine;

public class RankingGame : MonoBehaviour
{
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text gameTimeText;


    public void Init(GameInfo gameInfo)
    {
        dateText.text = gameInfo.gameDate.ToString();
        TimeSpan timeSpan = TimeSpan.FromSeconds(gameInfo.totalSeconds);
        gameTimeText.text = $"{(int)timeSpan.TotalHours}:{timeSpan.Minutes}:{timeSpan.Seconds}";
        gameObject.SetActive(true);
    }
}
