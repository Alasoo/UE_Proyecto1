using System;
using System.Collections.Generic;
using UnityEngine;
using Controller.Player;


[CreateAssetMenu(fileName = "NuevaRecompensa", menuName = "ScriptableObjects/Reward/AttackSpeed", order = 4)]
public class AddAttackSpeedScriptable : RewardScriptable
{
    public int attackSpeedToAdd = 10;

    public override void SelectReward(PlayerStats player)
    {
        player.AddAttackSpeed(attackSpeedToAdd);
    }

    public override (string lastValue, string newValue) ShowReward(PlayerStats player)
    {
        string oldVal = player.attackSpeedPercent.ToString() + "%";
        string newVal = (player.attackSpeedPercent + attackSpeedToAdd).ToString() + "%";
        return (oldVal, newVal);
    }
}


