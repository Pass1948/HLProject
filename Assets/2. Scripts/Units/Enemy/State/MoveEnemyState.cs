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
        Vector3Int start = controller.GridPos;
        Vector3Int dest = controller.TargetPos;
        List<Vector3Int> path = GameManager.Map.FindPath(start, dest);
        
        if (path == null || path.Count == 0)
        {
            stateMachine.ChangeState(stateMachine.EndState);
            return;
        }

        if (path[path.Count - 1] == dest && GameManager.Map.IsPlayer(dest) || GameManager.Map.IsVehicle(dest))
            path.RemoveAt(path.Count - 1);

        int range = Mathf.Min(controller.moveRange, path.Count);

        if (range > 0)
        {
            Vector3 nextPos = GameManager.Map.tilemap.GetCellCenterWorld(path[0]);

            animHandler.OnMove(true, nextPos);
            controller.StartCoroutine(MoveAnim(path.GetRange(0, range)));
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
        animHandler.OnMove(false, controller.transform.position);
        GameManager.Map.ClearPlayerRange();
    }

    // Ÿ��(�÷��̾�)���� �Ÿ� ���
    private int GetDistanceTarget(Vector3Int pos, Vector3Int target)
    {
        return Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y);
    }
    
    private IEnumerator MoveAnim(List<Vector3Int> path)
    {
        Vector3Int oldPos = controller.GridPos; 
        if (oldPos.x >= 0 && oldPos.y >= 0 && oldPos.x < GameManager.Map.mapWidth && oldPos.y < GameManager.Map.mapHeight)
        {
            int oldTileID = GameManager.Map.mapData[oldPos.x, oldPos.y];
            Debug.Log($"몬스터가 떠난 이전 좌표: ({oldPos.x}, {oldPos.y}) | 타일 ID: {oldTileID}");
        }
        
        yield return controller.MoveAlongPath(path);
        
        Vector3Int newPos = path[path.Count - 1];
        
        controller.GridPos = newPos;
        
        GameManager.Map.UpdateObjectPosition(oldPos, oldPos, newPos, newPos, TileID.Enemy);
        GameManager.Map.pathfinding.ResetMapData();
        controller.transform.position = GameManager.Map.tilemap.GetCellCenterWorld(newPos);
        
        stateMachine.ChangeState(stateMachine.EndState);
    }
    
    
}
