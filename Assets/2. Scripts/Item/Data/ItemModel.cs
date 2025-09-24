using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemModel // 데이터 바인딩용
{
    public int id;
    public ItemType itemType;
    public string name;
    public RarityType rarity;
    public string description;
    public int addAttack;
    public int addAttackRange;
    public int addMoveRange;
    public int addMaxHealth;
    public int addMulligan;
    public int addMaxBullet;
    public int moneyBonus;
    public int damageBonus;
    public int reducedDamage;
    public int addBikeHealth;
    public int bikeAddMoveRange;
    public bool conditionall;
    
    public void InitData(ItemData data) // 초기화용 메서드
    {
        id = data.ID;
        itemType = data.ItemType;
        name = data.name;
        rarity = data.Rarity;
        description = data.Description;
        addAttack = data.AddAttack;
        addAttackRange = data.AddAttackRange;
        addMoveRange = data.AddMoveRange;
        addMaxHealth = data.AddMaxHealth;
        addMulligan = data.AddMulligan;
        addMaxBullet = data.AddMaxBullet;
        moneyBonus = data.MoneyBonus;
        damageBonus = data.DamageBonus;
        reducedDamage = data.ReducedDamage;
        addBikeHealth = data.AddBikeHealth;
        bikeAddMoveRange = data.BikeAddMoveRange;
        conditionall = data.Conditionall;
    }
}

