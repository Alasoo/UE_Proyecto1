using System;
using System.Collections.Generic;
using Controller.Player;
using UnityEngine;
using UnityEngine.Localization;


public abstract class RewardScriptable : ScriptableObject
{
    //LocalizeStringEvent
    public LocalizedString localization;
    public abstract void SelectReward(PlayerStats player);
    public abstract (string lastValue, string newValue) ShowReward(PlayerStats player);
}


