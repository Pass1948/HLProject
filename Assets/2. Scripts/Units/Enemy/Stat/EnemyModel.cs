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

// [UGS(typeof(EnemyType))]
public enum EnemyType
{
    Normal,
    Elite,
}

public class EnemyModel : UnitModel
{
    public EnemyType enemyType;
    public EnemyAttribute attri;
    public int rank;
    public int attack;
    public int minAttackRange;
    public int maxAttackRange;

    public bool isDie = false;
    public bool isDone = true;

    public void InitData(EntityData data, EnemyType enemyType)
    {
        unitType = data.type;
        enemyType = enemyType;
        id = data.id;
        size = data.size;
        unitName = data.name;
        attri = data.attribute;
        rank = Random.Range(data.minNum, data.maxNum);
        maxHealth = 10;
        currentHealth = maxHealth;
        attack = data.attack;
        minAttackRange = data.minAttackRange;
        maxAttackRange = data.minAttackRange;
        moveRange = data.moveRange;
    }
}

