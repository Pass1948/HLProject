using DataTable;
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
    public int baseMoveRange;
    public int moveRange;
    public int mulligan;
    public int reload;
    public int baseHealth;
    public int health;
    public int monney = 10;
    public bool die => currentHealth <= 0;

    public void InitData(EntityData data)
    {
        unitType = data.type;
        id = data.id;
        unitName = data.name;
        size = data.size;
        attack = data.attack;
        attackRange = Random.Range(data.minAttackRange, data.maxAttackRange);
        moveRange = data.moveRange;
        maxHealth = data.health;
        health= maxHealth;
        currentHealth = health;
        mulligan = data.mulligan;
        reload = data.reload;
    }

}
