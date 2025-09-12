using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideEnemyState : BaseEnemyState
{
    public DecideEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        Dictionary<IEnemyState, int> actionScores = new Dictionary<IEnemyState, int>();

        // 무슨 행동을 할지 점수를 매겨서 결정하자 -> 일반몬스터는 이동 / 공격
        // 엘리트 -> 이동 / 공격 / + 다른거
        // 보스 -> 이동 / 공격 / + 여러 패턴들

        int distance = GetDistanceTarget(stateMachine.Controller.GridPos, stateMachine.Controller.TargetPos);

        //if (distance <= stateMachine.Controller.AttackRange)
        //{
        //    stateMachine.ChangeState(stateMachine.AttackState);
        //}
        //else
        //{
            stateMachine.ChangeState(stateMachine.MoveState);
        //}
    }

    public override void Excute()
    {
        Debug.Log("Decide : Excute");
    }

    public override void Exit()
    {
        Debug.Log("Decide : Exit");
    }

    // 타겟(플레이어)와의 거리 계산
    private int GetDistanceTarget(Vector3Int pos, Vector3Int target)
    {
        return Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y);
    }
}
