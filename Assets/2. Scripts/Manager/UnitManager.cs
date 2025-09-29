using System.Collections;
using System.Collections.Generic;
using GoogleSheet.Core.Type;
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
                    if(enemy.rank == 1 && ammo.rank == 13)
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

}
