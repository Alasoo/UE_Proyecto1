using System;
using System.Collections.Generic;
using UnityEngine;
using Controller.Player;


[CreateAssetMenu(fileName = "NuevaRecompensa", menuName = "ScriptableObjects/Reward/SpeedMovement", order = 4)]
public class AddMovementSpeedScriptable : RewardScriptable
{
    public int speedPercentToAdd = 10;

    public override void SelectReward(PlayerStats player)
    {
        player.AddMovementSpeed(speedPercentToAdd);
    }

    public override (string lastValue, string newValue) ShowReward(PlayerStats player)
    {
        string oldVal = player.movementSpeedPercent.ToString()+ "%";
        string newVal = (player.movementSpeedPercent + speedPercentToAdd).ToString()+ "%";
        return (oldVal, newVal);
    }
}


