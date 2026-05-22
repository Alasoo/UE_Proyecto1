using System;
using System.Collections.Generic;
using UnityEngine;
using Controller.Player;


[CreateAssetMenu(fileName = "NuevaRecompensa", menuName = "ScriptableObjects/Reward/MaxHeal", order = 4)]
public class AddHealScriptable : RewardScriptable
{
    public int maxHealToAdd = 20;

    public override void SelectReward(PlayerStats player)
    {
        player.AddMaxHealth(maxHealToAdd);
    }

        public override (string lastValue, string newValue) ShowReward(PlayerStats player)
    {
        string oldVal = player.maxHealth.ToString();
        string newVal = (player.maxHealth + maxHealToAdd).ToString();
        return (oldVal, newVal);
    }
}


