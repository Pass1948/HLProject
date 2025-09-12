using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [Header("Player Movement Info")]
    [field: SerializeField][Range(0, 10)] public int MoveRange = 3;
    [field: SerializeField][Range(0, 10)] public int MoveRangeModifier = 3;


    [Header("Player Attack Info")]
    [field: SerializeField][Range(0, 10)] public int AttackRange = 7;
    [field: SerializeField][Range(0, 10)] public int AttackRangeModifier = 10;
    [field: SerializeField][Range(0, 10)] public int AttackDamage;
    [field: SerializeField][Range(0, 10)] public int AttackDamageModifier;
    [field: SerializeField][Range(0, 20)] public int BulletCount;

    [Header("Player Condition Info")]
    [field: SerializeField][Range(0, 10)] public int StunTurn;
    [field: SerializeField][Range(0, 10)] public int PoisonTurn;
    [field: SerializeField][Range(0, 10)] public int BurnTurn;
    [field: SerializeField][Range(0, 10)] public float HP = 100;
}
