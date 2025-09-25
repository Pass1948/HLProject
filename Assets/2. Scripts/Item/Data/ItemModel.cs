using System.Collections;
using System.Collections.Generic;
using DataTable;
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
    public int conditionall;
    public string path;
    public void InitData(RelicData data) // 초기화용 메서드
    {
        id = data.id;
        itemType = data.itemType;
        name = data.name;
        rarity = data.rarityType;
        description = data.descript;
        addAttack = data.addAttack;
        addAttackRange = data.addAttackRange;
        addMoveRange = data.addMoveRange;
        addMaxHealth = data.addHealth;
        addMulligan = data.addMulligan;
        addMaxBullet = data.addMaxBullet;
        moneyBonus = data.moneyBonus;
        damageBonus = data.damageBonus;
        reducedDamage = data.reducedDamage;
        addBikeHealth = data.addBikeHealth;
        bikeAddMoveRange = data.bikeAdditinal;
        conditionall = data.conditionall;
       // path =  data.path;
    }
}

