using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluateEnemyState : BaseEnemyState
{
    // Enemy에 대한 턴이 시작되었을 때 전환할 상태
     
    public EvaluateEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {

        if(GameManager.Unit.Player.playerModel.die)
        {
            stateMachine.ChangeState(stateMachine.EndState);
            return;
        }

        stateMachine.ChangeState(stateMachine.DecideState);

    }

    public override void Excute()
    {

        // Do nothing!!
    }

    public override void Exit()
    {
    }


    // 타겟(플레이어)와의 거리 계산
    private int GetDistanceTarget(Vector3Int pos, Vector3Int target)
    {
        return Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y);
    }


}
