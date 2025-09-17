using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ViecleBording
{
    None,
    On,
    off,
}
// TODO : 나중에 오토바이 체력을 합칠꺼라면 맥스 체력도 만들어야 하나? ,무브레인지도(JBS)
public class PlayerModel : UnitModel
{
    public ViecleBording viecleBording;
    public int attack;
    public int attackRange;
    public int moveRange = 1;
    public int mulligan;
    public int reload;
    public int health;
    public bool die => health <= 0; //0이하면 트루
    public void InitData(UnitData data)
    {
        unitType = data.Type;
        id = data.ID;
        unitName = data.Name;
        size = data.Size;
        attack = data.Attack;
        attackRange = Random.Range(data.MinAttackRange, data.MaxAttackRange);
        moveRange = data.MoveRange;
        maxHealth = data.Health;
        currentHealth = maxHealth;
        mulligan = data.Mulligan;
        reload = data.Reload;
    }
}
