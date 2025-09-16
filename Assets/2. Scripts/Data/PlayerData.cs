using UnityEngine;
using System;

[Serializable]
public class PlayerMoveData
{
    [Header("플레이어 이동Info")]
    [field: SerializeField] public Vector3Int PlayerPos;
    [field: SerializeField] public int MoveRange =1;
    [field: SerializeField] public int MoveRangeModifier =2;

    [field: SerializeField][Range(0, 10)] public float HP = 100;
}

[Serializable]
public class PlayerAttackData
{
    [Header("플레이어 공격Info")]
    [field: SerializeField][Range(0, 10)] public int AttackRange = 7;
    [field: SerializeField][Range(0, 10)] public int AttackRangeModifier = 10;
    [field: SerializeField][Range(0, 10)] public int AttackDamage;
    [field: SerializeField][Range(0, 10)] public int AttackDamageModifier;
    [field: SerializeField][Range(0, 20)] public int BulletCount;
}

[Serializable]
public class PlayerConditionData
{
    [Header("플레이어 상태Info")]
    [field: SerializeField][Range(0, 10)] public int StunTurn;
    [field: SerializeField][Range(0, 10)] public int PoisonTurn;
    [field: SerializeField][Range(0, 10)] public int BurnTurn;
    [field: SerializeField][Range(0, 10)] public int FreezeTurn;
}

// 이 애들을 불러서
public class PlayerData
{
    [Header("플레이어Data")]
    [field: SerializeField] public PlayerMoveData playerMoveData;
    [field: SerializeField] public PlayerAttackData playerAttackData;
    [field: SerializeField] public PlayerConditionData playerConditionData;
}

