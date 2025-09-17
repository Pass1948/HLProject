using System;
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

        List<Vector3Int> path = GameManager.Map.FindPath(enemyPos, playerPos);

        if (path == null || path.Count == 0)
        {
            Debug.Log("노 이동");
            stateMachine.ChangeState(stateMachine.EndState);
            return;
        }

        if (path[path.Count - 1] == playerPos && GameManager.Map.IsPlayer(playerPos))
        {
            path.RemoveAt(path.Count - 1);
        }


        GameManager.Map.PlayerUpdateRange(controller.GridPos, controller.moveRange);

        controller.StartCoroutine(PreviewPath(path));


       
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

    private IEnumerator PreviewPath(List<Vector3Int> path)
    {
        yield return new WaitForSeconds(1f);

        GameManager.Map.ClearPlayerRange();

        int distance = GetDistanceTarget(controller.GridPos, controller.TargetPos);
        if (distance >= controller.minAttackRange && distance <= controller.maxAttackRange)
        {
            stateMachine.ChangeState(stateMachine.AttackState);
        }
        else
        {
            stateMachine.ChangeState(stateMachine.MoveState);
        }
    }
}
