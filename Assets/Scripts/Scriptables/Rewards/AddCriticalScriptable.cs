using System;
using System.Collections.Generic;
using UnityEngine;
using Controller.Player;


[CreateAssetMenu(fileName = "NuevaRecompensa", menuName = "ScriptableObjects/Reward/Critical", order = 4)]
public class AddCriticalScriptable : RewardScriptable
{
    public int criticalToAdd = 5;

    public override void SelectReward(PlayerStats player)
    {
        player.AddCritical(criticalToAdd);
    }

    public override (string lastValue, string newValue) ShowReward(PlayerStats player)
    {
        string oldVal = player.criticalPercent.ToString() + "%";
        int criticalFix = Mathf.Min(player.criticalPercent + criticalToAdd, 100);
        string newVal = criticalFix.ToString() + "%";
        return (oldVal, newVal);
    }
}


