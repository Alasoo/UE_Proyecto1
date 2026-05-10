using System;
using System.Collections.Generic;
using UnityEngine;
using Controller.Player;


[CreateAssetMenu(fileName = "NuevaRecompensa", menuName = "ScriptableObjects/Reward/Projectile", order = 4)]
public class AddProjectileScriptable : RewardScriptable
{
    public int projectilesToAdd = 1;

    public override void SelectReward(PlayerStats player)
    {
        player.AddProjectiles(projectilesToAdd);
    }

    public override (string lastValue, string newValue) ShowReward(PlayerStats player)
    {
        string oldVal = player.currentProjectiles.ToString();
        string newVal = (player.currentProjectiles + projectilesToAdd).ToString();
        return (oldVal, newVal);
    }
}


