using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum VehicleCondition
{
    Riding,
    GetOff,
    Destruction,// �ı�
    Repair, // ����
}   

// TODO: ���ڵ��� �׽�Ʈ�� ���� �ӽ÷� �־��ִ� �ͻ��Դϴ�~(JBS)
public class VehicleModel : UnitModel
{
    public int maxBullet;
    public int additinalMove;
    public int additinalHealth;
    public int moveRange = 2;

    public VehicleCondition condition;

    public bool isDestruction => condition == VehicleCondition.Destruction;


    public int health = 3;

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
