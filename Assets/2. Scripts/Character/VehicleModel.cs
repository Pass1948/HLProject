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

    public void InitData(UnitData data)
    {
        unitType = data.Type;
        id = data.ID;
        unitName = data.Name;
        size = data.Size;
        moveRange = data.MoveRange;
        maxHealth = data.Health;
        currentHealth = maxHealth;
        maxBullet = data.MaxBullet;
    }
}
