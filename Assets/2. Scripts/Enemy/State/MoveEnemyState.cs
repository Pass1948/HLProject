using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemyState : BaseEnemyState
{
    public MoveEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public Pathfinding pathfind;

    public override void Enter()
    {
        Debug.Log("Move : Enter");

        Vector3Int start = controller.GridPos;
        Vector3Int dest = controller.TargetPos;

        Debug.Log(dest);
        Debug.Log($"MoveRange : {controller.moveRange}");

        List<Vector3Int> path = GameManager.Map.FindPath(start, dest);

        GameManager.PathPreview.ShowPath(path, GameManager.Map.tilemap, GameManager.Unit.enemies[0].enemyModel.moveRange);
        
        if (path == null || path.Count == 0)
        {
            Debug.Log("길찾기 모담");
            stateMachine.ChangeState(stateMachine.EndState);
            return;
        }

        int range = Mathf.Min(controller.moveRange, path.Count);

        controller.StartCoroutine(MoveAnim(path.GetRange(0, range)));
        animHandler.OnMove(true);
    }

    public override void Excute()
    {
        Debug.Log("Move : Excute");

    }

    public override void Exit()
    {
        Debug.Log("Move : Exit");
        animHandler.OnMove(false);

    }

    // 타겟(플레이어)와의 거리 계산
    private int GetDistanceTarget(Vector3Int pos, Vector3Int target)
    {
        return Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y);
    }

    private IEnumerator MoveAnim(List<Vector3Int> path)
    {
        yield return controller.MoveAlongPath(path);
        Debug.Log(path.Count);
        Vector3Int last = path[path.Count - 1];
        controller.GridPos = last;

        GameManager.Map.UpdateObjectPosition(controller.GridPos.x, controller.GridPos.y, last.x, last.y, TileID.Enemy);

        controller.transform.position = GameManager.Map.tilemap.GetCellCenterWorld(last);

        // 이동 후 공격 가능 여부 확인
        int distance = GetDistanceTarget(controller.GridPos, controller.TargetPos);
        if (distance <= controller.attackRange)
        {
            stateMachine.ChangeState(stateMachine.AttackState);

        }
        else
        {
            stateMachine.ChangeState(stateMachine.EndState);
        }
    }


}
