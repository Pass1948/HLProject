using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;


public enum VehicleCondition
{
    Riding,
    GetOff,
    Destruction,// 파괴
    Repair, // 수리
}   

// TODO: 숫자는 테스트 용으로 넣은겁니다~(JBS)
public class VehicleModel : UnitModel
{
    public int maxBullet;
    public int additinalMove;
    public int additinalHealth;

    public VehicleCondition condition;

    public bool isDestruction => condition == VehicleCondition.Destruction;


    public int health = 3;

    public void InitData(EntityData data)
    {
        unitType = data.type;
        id = data.id;
        unitName = data.name;
        size = data.size;
        moveRange = data.moveRange;
        baseMoveRange = data.moveRange;
        maxHealth = data.health;
        currentHealth = maxHealth;
        maxBullet = data.maxBullet;
    }

}
