using UnityEngine;

public class PlayerData
{
    [Header("플레이어 이동Info")]
    [field: SerializeField][Range(0f, 10f)] private int MoveRange;
    [field: SerializeField][Range(0f, 10f)] private int MoveRangeModifier;

    [Header("플레이어 공격Info")]
    [field: SerializeField][Range(0f, 10f)] private int AttackRange;
    [field: SerializeField][Range(0f, 10f)] private int AttackRangeModifier;
    [field: SerializeField][Range(0f, 10f)] private int AttackDamage;
    [field: SerializeField][Range(0f, 10f)] private int AttackDamageModifier;

    
    [field: SerializeField][Range(0f, 10f)] private float HP;

    [field: SerializeField][Range(0f, 20f)] private int BulletCount;



}

