using GoogleSheet.Core.Type;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[UGS(typeof(UnitType))]
public enum UnitType
{
    Player,
    Enemy,
    Vehicle,
    Obstacle,
    Boss,
    Unknown
}

public class UnitManager : MonoBehaviour
{
    public BasePlayer _player;
    public BasePlayer Player { get { return _player; } set { _player = value; } }

    public BaseVehicle _vehicle;
    public BaseVehicle Vehicle { get { return _vehicle; } set { _vehicle = value; } }

    public List<BaseEnemy> enemies = new List<BaseEnemy>();

    public BaseBoss boss = null;

   public bool isInit = false;

    // =====[���� ���� ����� ����]=====
    public int curAttack;
    public int curAttackRange;
    public int curMoveRange;
    public int curMulligan;
    public int curHealth;

    // =====[���� ���� ����� ����]=====
    public int curVMoveRange;
    public int curVHealth;

    public void ChangeHealth(UnitModel unit, int damage, Ammo ammo = null)
    {
        if (unit.unitType == UnitType.Player)
        {
            Player.playerHandler.TakeDamage(damage);
        }
        else if (unit.unitType == UnitType.Enemy)
        {
            EnemyModel enemy = (EnemyModel)unit;

            switch (enemy.attri)
            {
                case EnemyAttribute.High:
                    if (enemy.rank == 13 && ammo.rank == 1)
                    {
                        unit.currentHealth -= damage;
                    }
                    else if (enemy.rank < ammo.rank)
                    {
                        unit.currentHealth -= damage;
                    }
                    break;
                case EnemyAttribute.Low:
                    if (enemy.rank == 1 && ammo.rank == 13)
                    {
                        unit.currentHealth -= damage;
                    }
                    else if (enemy.rank > ammo.rank)
                    {
                        unit.currentHealth -= damage;
                    }
                    break;
            }
        }
    }

    public void CurrentStatReset()
    {
        curAttack = Player.playerModel.attack;
      //curAttackRange = Player.playerModel.attackRange;
        curMoveRange = Player.playerModel.moveRange;
        curMulligan = Player.playerModel.mulligan;
        curHealth = Player.playerModel.currentHealth;
        curVHealth = Vehicle.vehicleModel.currentHealth;
        isInit = true;
        Debug.Log($"플레이어 체력{curHealth}<= {Player.playerModel.currentHealth}");
    }

    public void SetCurrentStat()
    {
        Player.playerModel.attack =curAttack;
       // Player.playerModel.attackRange = curAttackRange;
        Player.playerModel.moveRange = curMoveRange;
        Player.playerModel.mulligan = curMulligan;
        Player.playerModel.currentHealth = curHealth;
        Vehicle.vehicleModel.currentHealth = curVHealth;
        isInit  = false;
        Debug.Log($"플레이어 체력{Player.playerModel.currentHealth}<= {curHealth}");
    }

    public void AllClearEnemies()
    {
        foreach (var enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
    }

}
