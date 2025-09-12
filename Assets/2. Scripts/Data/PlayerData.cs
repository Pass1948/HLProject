using UnityEngine;

public class PlayerData
{
    [Header("�÷��̾� �̵�Info")]
    [field: SerializeField][Range(0f, 10f)] private int MoveRange;
    [field: SerializeField][Range(0f, 10f)] private int MoveRangeModifier;

    [Header("�÷��̾� ����Info")]
    [field: SerializeField][Range(0f, 10f)] private int AttackRange;
    [field: SerializeField][Range(0f, 10f)] private int AttackRangeModifier;
    [field: SerializeField][Range(0f, 10f)] private int AttackDamage;
    [field: SerializeField][Range(0f, 10f)] private int AttackDamageModifier;

    
    [field: SerializeField][Range(0f, 10f)] private float HP;

    [field: SerializeField][Range(0f, 20f)] private int BulletCount;



}

