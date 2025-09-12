using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluateEnemyState : BaseEnemyState
{
    // Enemy에 대한 턴이 시작되었을 때 전환할 상태
     
    public EvaluateEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        Debug.Log("Evaluate : Enter");

        // 행동 결정 후
        // 행동에 맞는 상태로 전환
        // 예시
        // if (공격 범위 안으로 들어왔을 경우)
           //{
           //     // 공격 상태 전환
           //    stateMachine.ChangeState(stateMachine.AttackState);
           //}
           //else  // 아닐경우
           //{
           //     // 이동으로 전환
           //    stateMachine.ChangeState(stateMachine.MoveState);
           //} 
        
               stateMachine.ChangeState(stateMachine.DecideState);

        //int distance = GetDistanceTarget()

    }

    public override void Excute()
    {
        Debug.Log("Evaluate : Excute");

        // Do nothing!!
    }

    public override void Exit()
    {
        Debug.Log("Evaluate : Exit");
    }


    // 타겟(플레이어)와의 거리 계산
    private int GetDistanceTarget(Vector2Int pos, Vector2Int target)
    {
        return Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y);
    }


}
