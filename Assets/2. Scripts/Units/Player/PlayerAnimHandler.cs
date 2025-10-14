using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimHandler : MonoBehaviour
{
   [SerializeField] Animator animator;

    public void PlayMoveAnim()
    {
        if (animator.GetBool("IsMoving") == false)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    public void PlayerAttackAnim()
    {
        animator.SetTrigger("IsAttack");
    }

    public void PlayerKickAnim()
    {
        animator.SetTrigger("IsKick");
    }

}
