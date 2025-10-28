using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluateBossState : BaseBossState
{
    public EvaluateBossState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler)
        : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        // 쿨타임 만들기
        Debug.Log("Evaluate boss state");
        if(GameManager.Unit.Player.playerModel.die)
        {
            stateMachine.ChangeState(stateMachine.EndState);
            return;
        }

        if (controller.isStun)
        {
            Debug.Log("보스 스턴 중 공격 불가");
            stateMachine.ChangeState(stateMachine.EndState);
            return;
        }

        stateMachine.ChangeState(stateMachine.DecideState);
    }

    public override void Exit()
    {
  
    }

    public override void Excute()
    {

    }
    
    private int GetDistanceTarget(Vector3Int pos, Vector3Int target)
    {
        return Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y);
    }
}
