using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Player,
    Enemy,
    Vehicle,
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
                    if (enemy.rank < ammo.rank)
                    {
                        Debug.Log($"몬스터의 체력은(enemy.currentHealth) : {enemy.currentHealth},몬스터의 체력은(unit.currentHealth){unit.currentHealth}");
                        unit.currentHealth -= damage;
                        Debug.Log($"몬스터의 체력은(enemy.currentHealth) : {enemy.currentHealth},몬스터의 체력은(unit.currentHealth){unit.currentHealth}");

                    }
          
                    break;
                case EnemyAttribute.Low:
                    if (enemy.rank > ammo.rank)
                    {
                        Debug.Log($"몬스터의 체력은(enemy.currentHealth) : {enemy.currentHealth},몬스터의 체력은(unit.currentHealth){unit.currentHealth}");
                        unit.currentHealth -= damage;
                        Debug.Log($"몬스터의 체력은(enemy.currentHealth) : {enemy.currentHealth},몬스터의 체력은(unit.currentHealth){unit.currentHealth}");
                    }
                    break;
            }

        }
    }

}
