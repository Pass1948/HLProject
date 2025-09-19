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

        //GameManager.Map.PlayerUpdateRange(controller.GridPos, controller.moveRange);

        Vector3Int start = controller.GridPos;
        Vector3Int dest = controller.TargetPos;
        List<Vector3Int> path = GameManager.Map.FindPath(start, dest);

        //Debug.Log(dest);


        
        if (path == null || path.Count == 0)
        {
            Debug.Log("길찾기 모담");
            stateMachine.ChangeState(stateMachine.EndState);
            return;
        }

        if (path[path.Count - 1] == dest && GameManager.Map.IsPlayer(dest))
            path.RemoveAt(path.Count - 1);

        int range = Mathf.Min(controller.moveRange, path.Count);

        if (range > 0)
        {
            controller.StartCoroutine(MoveAnim(path.GetRange(0, range)));
            animHandler.OnMove(true);
        }
        else
        {
            stateMachine.ChangeState(stateMachine.EndState);
        }

    }

    public override void Excute()
    {

    }

    public override void Exit()
    {
        animHandler.OnMove(false);
        GameManager.Map.ClearPlayerRange();
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

        stateMachine.ChangeState(stateMachine.EndState);
    }
}
