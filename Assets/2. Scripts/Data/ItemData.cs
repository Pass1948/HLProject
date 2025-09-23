using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData : MonoBehaviour
{
    public int ID;
    public ItemType ItemType;
    public string Name;
    public RarityType Rarity;
    public string Description;
    public int AddAttack;
    public int AddAttackRange;
    public int AddMoveRange;
    public int AddMaxHealth;
    public int AddMulligan;
    public int AddMaxBullet;
    public int MoneyBonus;
    public int DamageBonus;
    public int ReducedDamage;
    public int AddBikeHealth;
    public int BikeAddMoveRange;
    public bool Conditionall;
}
