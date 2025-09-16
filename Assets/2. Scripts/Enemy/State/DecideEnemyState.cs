using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideEnemyState : BaseEnemyState
{
    public DecideEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {

        Vector3Int playerPos = controller.TargetPos;
        Vector3Int enemyPos = controller.GridPos;

        if (!GameManager.Map.IsPlayer(playerPos))
        {
            Debug.Log("플레이어 못찾음 : 엔드 상태로");
            Debug.Log(playerPos.x + ", " + playerPos.y);

            stateMachine.ChangeState(stateMachine.EndState);
            return;
        }

        int distance = GetDistanceTarget(enemyPos, playerPos);
        if (distance >= controller.minAttackRange && distance <= controller.maxAttackRange)
        {
            stateMachine.ChangeState(stateMachine.AttackState);
        }
        else
        {
            stateMachine.ChangeState(stateMachine.MoveState);
        }
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
