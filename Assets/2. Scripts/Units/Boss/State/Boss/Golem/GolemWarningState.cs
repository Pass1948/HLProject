using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemWarningState : WarningBossState
{
    public GolemWarningState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler) 
        : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        controller.StartCoroutine(ShowWarning());
    }

    private IEnumerator ShowWarning()
    {
        Vector3Int bossPos = controller.GridPos;
        Vector3Int playerPos = controller.TargetPos;
        Vector3Int dir = GetDirection(bossPos, playerPos);
        List<Vector3Int> range = GetForwardRange(bossPos, dir);

        map.attackRange.ShowRange(range);
        controller.warningCells = range;

        yield return new WaitForSeconds(1f);
        
        stateMachine.ChangeState(stateMachine.EndState);
    }

    private Vector3Int GetDirection(Vector3Int bossPos, Vector3Int playerPos)
    {
        int dx = playerPos.x - bossPos.x;
        int dy = playerPos.y - bossPos.y;

        if (Mathf.Abs(dx) > Mathf.Abs(dy))
            return (dx > 0) ? Vector3Int.right : Vector3Int.left;
        else
            return (dy > 0) ? Vector3Int.up : Vector3Int.down;
    }
    
    private List<Vector3Int> GetForwardRange(Vector3Int bossPos, Vector3Int dir)
    {
        List<Vector3Int> range = new List<Vector3Int>();
        List<Vector3Int> baseOffsets = new List<Vector3Int>()
        {
            new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0),
            new Vector3Int(-1, 2, 0), new Vector3Int(0, 2, 0), new Vector3Int(1, 2, 0),
            new Vector3Int(-1, 3, 0), new Vector3Int(0, 3, 0), new Vector3Int(1, 3, 0),
        };
        
        foreach (Vector3Int offset in baseOffsets)
        {
            range.Add(bossPos + RotationOffset(offset, dir));
        }
        
        return range;
    }

    private Vector3Int RotationOffset(Vector3Int offset, Vector3Int dir)
    {
        if (dir == Vector3Int.up) return new Vector3Int(offset.x, -offset.y, 0);
        if (dir == Vector3Int.down) return offset;
        if (dir == Vector3Int.left) return new Vector3Int(offset.y, offset.x, 0);
        if (dir == Vector3Int.right) return new Vector3Int(-offset.y, -offset.x, 0);
        return offset;
    }

    public override void Exit()
    {
        map.attackRange.ClearRange();
    }
}
