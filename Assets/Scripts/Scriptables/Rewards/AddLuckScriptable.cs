using System;
using System.Collections.Generic;
using UnityEngine;
using Controller.Player;


[CreateAssetMenu(fileName = "NuevaRecompensa", menuName = "ScriptableObjects/Reward/Luck", order = 4)]
public class AddLuckScriptable : RewardScriptable
{
    public int luckToAdd = 5;

    public override void SelectReward(PlayerStats player)
    {
        player.AddLuck(luckToAdd);
    }

    public override (string lastValue, string newValue) ShowReward(PlayerStats player)
    {
        string oldVal = player.luckPercent.ToString() + "%";
        string newVal = (player.luckPercent + luckToAdd).ToString() + "%";
        return (oldVal, newVal);
    }
}


