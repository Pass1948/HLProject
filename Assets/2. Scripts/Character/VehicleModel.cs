using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum VehicleCondition
{
    Bording,
    Destruction,// 파괴
    Repair, // 수리
}   

// TODO: 숫자들은 테스트를 위해 임시로 넣어주는 것뿐입니당~(JBS)
public class VehicleModel : UnitModel
{
    public int maxBullet;
    public int additinalMove;
    public int additinalHealth;
    public int moveRange = 2;
    public VehicleCondition condition = VehicleCondition.Bording;
    public bool isDestruction => condition == VehicleCondition.Destruction;


    public int health = 3;

    public void InitData(UnitData data)
    {
        id = data.ID;
        unitName = data.Name;
        size = data.Size;
        moveRange = data.MoveRange;
        health = data.Health;
        maxBullet = data.MaxBullet;
    }
}
