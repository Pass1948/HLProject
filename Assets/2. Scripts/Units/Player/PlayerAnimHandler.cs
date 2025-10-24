using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimHandler : MonoBehaviour
{
   [SerializeField] Animator animator;
   [SerializeField] float snap = 0.15f;          // 수평 우선 가중치(0~0.3 권장) 값이 클수록 = 좌/우로 더 잘 스냅
                                                 // 4방향 스냅 회전 (대각선 금지, 경계는 좌/우 우선)

    public void OnRiding()
    {
        if (GameManager.Unit.Vehicle.vehicleModel.condition == VehicleCondition.Riding)
        {
            animator.SetBool("IsRiding", true);
            GameManager.Unit.Vehicle.vehicleHandler.OnPositionForward();
        }
        else
        {
            animator.SetBool("IsRiding", false);
            GameManager.Unit.Vehicle.vehicleHandler.OnPositionForward();
        }
    }


    public void PlayMoveAnim(Transform target)
    {
        if (animator.GetBool("IsMoving") == false)
        {
            FaceToTarget4Dir(target);
            animator.SetBool("IsMoving", true);
            GameManager.Unit.Vehicle.vehicleHandler.OnPositionForward();
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    public void PlayerAttackAnim(Transform target)
    {
        this.transform.LookAt(target);
        animator.SetTrigger("IsAttack");
    }

    public void PlayerKickAnim(Transform target)
    {
        animator.SetTrigger("IsKick");
    }

    public void PlayerHitAnim()
    {
        animator.SetTrigger("IsHit");
    }
    
    // =====================================================================
    // 유틸
    // =====================================================================
    
    // 마우스포인터 방향으로 바라보는 로직
    // 상하좌우만 가능하며 경계가 애매할땐 좌, 우로 우선시함
    public void FaceToTarget4Dir(Transform target)  
    {
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        float ax = Mathf.Abs(dir.x); 
        float az = Mathf.Abs(dir.z);

        // 수평 우선적으로 상,화와 애매한 경계에서는 좌/우 쪽으로 더 쉽게 선택되도록함
        az /= (1f + snap);

        float angle;
        if (ax >= az)
            angle = (dir.x >= 0f) ? 90f : -90f;   // 우 / 좌
        else
            angle = (dir.z >= 0f) ? 0f  : 180f;   // 상 / 하

        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
    
}
