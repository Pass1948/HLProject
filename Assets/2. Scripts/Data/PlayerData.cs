using UnityEngine;
using System;

[Serializable]
public class PlayerMoveData
{
    [Header("�÷��̾� �̵�Info")]
    [field: SerializeField][Range(0, 10)] public int MoveRange = 3;
    [field: SerializeField][Range(0, 10)] public int MoveRangeModifier = 3;

    [field: SerializeField][Range(0, 10)] public float HP = 100;
}

[Serializable]
public class PlayerAttackData
{
    [Header("�÷��̾� ����Info")]
    [field: SerializeField][Range(0, 10)] public int AttackRange = 7;
    [field: SerializeField][Range(0, 10)] public int AttackRangeModifier = 10;
    [field: SerializeField][Range(0, 10)] public int AttackDamage;
    [field: SerializeField][Range(0, 10)] public int AttackDamageModifier;
    [field: SerializeField][Range(0, 20)] public int BulletCount;
}

[Serializable]
public class PlayerConditionData
{
    [Header("�÷��̾� ����Info")]
    [field: SerializeField][Range(0, 10)] public int StunTurn;
    [field: SerializeField][Range(0, 10)] public int PoisonTurn;
    [field: SerializeField][Range(0, 10)] public int BurnTurn;
    [field: SerializeField][Range(0, 10)] public int FreezeTurn;
}

// �� �ֵ��� �ҷ���
public class PlayerData
{
    [Header("�÷��̾�Data")]
    [field: SerializeField] public PlayerMoveData playerMoveData;
    [field: SerializeField] public PlayerAttackData playerAttackData;
    [field: SerializeField] public PlayerConditionData playerConditionData;
}

