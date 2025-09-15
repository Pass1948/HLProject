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

        Vector3Int enemyPos = controller.GridPos;
        Vector3Int playerPos = controller.TargetPos;

        if (!GameManager.Map.IsPlayer(playerPos))
        {
            Debug.Log("플레이어 못찾음 : 엔드 상태로");
            Debug.Log(playerPos.x + ", " + playerPos.y);

            stateMachine.ChangeState(stateMachine.EndState);
            return;
        }

        int distance = GetDistanceTarget(enemyPos, playerPos);
        if(distance <= controller.AttackRange)
        {
            stateMachine.ChangeState(stateMachine.AttackState);
        }
        else
        {
            stateMachine.ChangeState(stateMachine.DecideState);
        }

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
    private int GetDistanceTarget(Vector3Int pos, Vector3Int target)
    {
        return Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y);
    }


}
