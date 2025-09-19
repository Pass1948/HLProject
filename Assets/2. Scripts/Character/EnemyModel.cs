using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAttribute
{
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

    public void InitData(UnitData data)
    {
        unitType = data.Type;
        id = data.ID;
        size = data.Size;
        unitName = data.Name;
        attri = data.Attribute;
        rank = Random.Range(data.MinNum, data.MaxNum);
        maxHealth = 1;
        currentHealth = data.Health;
        attack = data.Attack;
        minAttackRange = data.MinAttackRange;
        maxAttackRange = data.MaxAttackRange;
        moveRange = data.MoveRange;
    }
}
