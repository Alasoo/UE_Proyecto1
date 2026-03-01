using System;
using Controller.Player;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NuevoPersonaje", menuName = "ScriptableObjects/Player", order = 1)]
public class PlayerDataScriptable : ScriptableObject
{
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