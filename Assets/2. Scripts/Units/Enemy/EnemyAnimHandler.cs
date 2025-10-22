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

    public void OnMove(bool isMove)
    {
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

    public void OnAttack()
    {
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
}
