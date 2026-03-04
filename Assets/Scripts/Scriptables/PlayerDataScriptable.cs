using System;
using Controller.Player;
using SaveSystem;
using SaveSystem.Player;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NuevoPersonaje", menuName = "ScriptableObjects/Player", order = 1)]
public class PlayerDataScriptable : ScriptableObject
{
    public string id;

    public LocalizedString playerName;
    [Space]
    public LocalizedString description;

    [Space(15)]
    public PlayerStateMachine playerStateMachine;

    [Space]
    [PreviewSprite(64)]
    public Sprite playerSprite;

    [Space]
    public PlayerCharacteristic playerCharacteristic;

    public int maxExperience = 10000;


    //public int experience = 0;
    //public int lvl = 1;
    public PlayerData playerData = new();



    public void LoadPlayerData()
    {
        var dataLoad = SaveLoadManager<PlayerData>.LoadData(id);
        if (!dataLoad.success) return;
        playerData = dataLoad.data;
    }
}

[Serializable]
public struct PlayerCharacteristic
{
    public float hpMax;
    public float manaMax;
    public float energyMax;

    public float physicalArmor;
    public float magicalArmor;
    public float physicalDamage;
    public float magicalDamage;
    public float speedMov;
}