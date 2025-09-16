using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : UnitModel
{
    public UnitType type;
    public int ID;
    public int Size;
    public int attack;
    public int attackRange;
    public int moveRange;
    public Vector3Int playerPos;
    public bool die;
    public int mulliganCnt;
    public int reloadCnt;
}
