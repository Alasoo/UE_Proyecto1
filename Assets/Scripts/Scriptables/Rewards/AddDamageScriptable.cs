using System;
using System.Collections.Generic;
using UnityEngine;
using Controller.Player;


[CreateAssetMenu(fileName = "NuevaRecompensa", menuName = "ScriptableObjects/Reward/Damage", order = 4)]
public class AddDamageScriptable : RewardScriptable
{
    public int damageToAdd = 10;

    public override void SelectReward(PlayerStats player)
    {
        player.AddDamage(damageToAdd);
    }

    public override (string lastValue, string newValue) ShowReward(PlayerStats player)
    {
        string oldVal = player.currentDamage.ToString();
        string newVal = (player.currentDamage + damageToAdd).ToString();
        return (oldVal, newVal);
    }
}


