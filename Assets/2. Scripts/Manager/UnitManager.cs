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
    public Player _player;
    public Player Player { get { return _player; } set { _player = value; } }

    public List<BaseEnemy> enemies = new List<BaseEnemy>();
    public void ChangeHealth(UnitModel unit, float health)
    {

    }


}
