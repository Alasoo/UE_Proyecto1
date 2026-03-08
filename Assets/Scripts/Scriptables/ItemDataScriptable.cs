using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;


[CreateAssetMenu(fileName = "NuevoItem", menuName = "ScriptableObjects/Item", order = 2)]
public class ItemDataScriptable : ScriptableObject
{
    public string id;
    public LocalizedString itemName;
    public int price;

    public PlayerDataScriptable character;
    public Slot slot = Slot.Weapon;     //por si quisieramos equipar en un futuro mas items con estadísticas


    [Space(15)]
    public ItemDamage itemDamage;

    [Space]
    [PreviewSprite(64)]
    public Sprite itemSprite;

}

[Serializable]
public struct ItemDamage
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