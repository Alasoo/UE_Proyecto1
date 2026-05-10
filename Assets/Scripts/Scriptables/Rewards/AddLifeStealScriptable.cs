using System;
using System.Collections.Generic;
using UnityEngine;
using Controller.Player;


[CreateAssetMenu(fileName = "NuevaRecompensa", menuName = "ScriptableObjects/Reward/LifeSteal", order = 4)]
public class AddLifeStealScriptable : RewardScriptable
{
    public int lifeStealPercentToAdd = 5;

    public override void SelectReward(PlayerStats player)
    {
        player.AddLifeSteal(lifeStealPercentToAdd);
    }

    public override (string lastValue, string newValue) ShowReward(PlayerStats player)
    {
        string oldVal = player.lifeStealPercent.ToString() + "%";
        string newVal = (player.lifeStealPercent + lifeStealPercentToAdd).ToString() + "%";
        return (oldVal, newVal);
    }
}


