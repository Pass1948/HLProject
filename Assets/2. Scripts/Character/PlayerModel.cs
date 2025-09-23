using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ViecleBording
{
    None,
    On,
    off,
}
// TODO : 숫자는 임시로 지정 해 놓은 값입니다(JBS)
public class PlayerModel : UnitModel
{
    public ViecleBording viecleBording;
    public int attack;
    public int attackRange;
    public int baseMoveRange = 1;
    public int moveRange = 1;
    public int mulligan;
    public int reload;
    public int baseHealth;
    public int health;
    public bool die => health <= 0; //0���ϸ� Ʈ��
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
