using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum VehicleCondition
{
    Bording,
    Destruction,// ÆÄ±«
    Repair, // ¼ö¸®
}   
public class VehicleModel : UnitModel
{
    public int maxBullet;
    public int additinalMove;
    public int additinalHealth;
    public VehicleCondition condition = VehicleCondition.Bording;
    public bool isDestruction => condition == VehicleCondition.Destruction;

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
