using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimHandler : MonoBehaviour
{
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Warning = Animator.StringToHash("Warning");
    private static readonly int Pattern = Animator.StringToHash("Pattern");
    private static readonly int Stun = Animator.StringToHash("Stun");

    [SerializeField] private Animator animator;

    [SerializeField] float snap = 0.15f;

    [SerializeField] public Transform modelTransform;

    public float rotationOffsetY = 0f;
    
    public void OnMove(bool isMove, Vector3 target)
    {
        FaceToTarget4Dir(target - transform.position);
        animator.SetBool(Move, isMove);
    }

    public void OnDie()
    {
        animator.SetBool(Die, true);
    }

    public void OnHit()
    {
        animator.SetTrigger(Hit);
    }

    public void OnAttack(Transform target)
    {
        this.transform.LookAt(target);
        animator.SetTrigger(Attack);
    }

    public void OnWarning()
    {
        animator.SetTrigger(Warning);
    }

    public void OnPattern()
    {
        animator.SetTrigger(Pattern);
    }

    public void OnStun(bool isStun)
    {
        animator.SetBool(Stun, isStun);
    }
    
    public void FaceToTarget4Dir(Vector3 direction)
    {
        direction.y = 0f;
        if (direction == Vector3.zero) return;
        
        float ax = Mathf.Abs(direction.x);
        float az = Mathf.Abs(direction.z);
        az /= (1f + snap);

        float angle;
        if (ax >= az)
            angle = (direction.x >= 0f) ? 90f : -90f;   // 우 / 좌
        else
            angle = (direction.z >= 0f) ? 0f  : 180f;   // 상 / 하

        modelTransform.transform.rotation = Quaternion.Euler(0f, angle + 180f, 0f);
    }
}
