using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData : MonoBehaviour   // 구글엑셀 시트의 왼쪽->오른쪽 순으로 입력함
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
