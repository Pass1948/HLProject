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
    public void ChangeHealth(UnitModel unit, float health)
    {
        Debug.Log($"ChangeHealth {unit.unitName} : {health}");
    }


}
