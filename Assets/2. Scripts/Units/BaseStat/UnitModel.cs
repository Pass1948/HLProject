using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitModel
{
    public UnitType unitType;
    public int id;
    public string unitName;
    public int size;
    public int maxHealth;
    public int currentHealth;
    public int baseMoveRange;
    public int moveRange;
    public bool die;
}
