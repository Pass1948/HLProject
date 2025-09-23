using System.Collections;
using System.Collections.Generic;
using DataTable;
using GoogleSheet.Core.Type;
using UnityEngine;

[UGS(typeof(EnemyAttribute))]
public enum EnemyAttribute
{
    None,
    Low,
    High
}

public class EnemyModel : UnitModel
{
    public EnemyAttribute attri;
    public int rank;
    public int attack;
    public int minAttackRange;
    public int maxAttackRange;

    public bool isDie = false;
    public bool isDone = true;

    public void InitData(EntityData data)
    {
        unitType = data.type;
        id = data.id;
        size = data.size;
        unitName = data.name;
        attri = data.attribute;
        rank = Random.Range(data.minNum, data.maxNum);
        maxHealth = data.health;
        currentHealth = maxHealth;
        attack = data.attack;
        minAttackRange = data.minAttackRange;
        maxAttackRange = data.minAttackRange;
        moveRange = data.moveRange;
    }
}

