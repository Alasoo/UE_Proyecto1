using System;
using System.Collections.Generic;
using UnityEngine;
using Controller.Player;


[CreateAssetMenu(fileName = "NuevaRecompensa", menuName = "ScriptableObjects/Reward/Stun", order = 4)]
public class StunProjectileScriptable : RewardScriptable
{
    public float timeStun = .1f;

    public override void SelectReward(PlayerStats player)
    {
        player.AddStunTime(timeStun);
    }

    public override (string lastValue, string newValue) ShowReward(PlayerStats player)
    {
        string oldVal = player.stunTime.ToString();
        string newVal = (player.stunTime + timeStun).ToString();
        return (oldVal, newVal);
    }
}


