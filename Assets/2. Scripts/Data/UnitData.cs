using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitData
{
    public UnitType Type;
    public int ID;
    public string Name;
    public int Size;
    public int Attack;
    public int BaseMoveRange;
    public int BaseHealth;
    public int MinAttackRange;
    public int MaxAttackRange;
    public int MoveRange;
    public int Health;
    public EnemyAttribute Attribute;
    public int MaxNum;
    public int MinNum;
    public int Mulligan;
    public int Reload;
    public int MaxBullet;
    public int Additional;
}
