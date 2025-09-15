using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAttribute
{
    Low,
    High
}

public class EnemyModel : UnitModel
{
    public EnemyAttribute attribute;
    public int number;
    public int attack;
    public int attackRange;

}
