using System;
using System.Collections.Generic;
using UnityEngine;
using Controller.Player;


[CreateAssetMenu(fileName = "NuevaRecompensa", menuName = "ScriptableObjects/Reward/Armor", order = 4)]
public class AddArmorScriptable : RewardScriptable
{
    public int armorToAdd = 20;

    public override void SelectReward(PlayerStats player)
    {
        player.AddArmor(armorToAdd);
    }

        public override (string lastValue, string newValue) ShowReward(PlayerStats player)
    {
        string oldVal = player.armor.ToString();
        string newVal = (player.armor + armorToAdd).ToString();
        return (oldVal, newVal);
    }
}


