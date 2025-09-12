using UnityEngine;
using System;

[Serializable]
public class PlayerMoveData
{
    [Header("플레이어 이동Info")]
    [field: SerializeField][Range(0, 10)] public int MoveRange;
    [field: SerializeField][Range(0, 10)] public int MoveRangeModifier;

    [field: SerializeField][Range(0, 10)] public float HP;
}
[Serializable]
public class PlayerAttackData
{
       [Header("플레이어 공격Info")]
    [field: SerializeField][Range(0, 10)] public int AttackRange;
    [field: SerializeField][Range(0, 10)] public int AttackRangeModifier;
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

public class PlayerData : MonoBehaviour
{
    [Header("플레이어Data")]
    [field: SerializeField] public PlayerMoveData playerMoveData;
    [field: SerializeField] public PlayerAttackData playerAttackData;
    [field: SerializeField] public PlayerConditionData playerConditionData;
}

