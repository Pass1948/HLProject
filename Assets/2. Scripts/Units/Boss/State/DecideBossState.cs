using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideBossState : BaseBossState
{
    public DecideBossState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler) 
        : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        // 플레이어가 패턴 범위 안에 있고 패턴 사용이 가능하다 -> 경고
        // 경고를 한 이후 -> 패턴 공격
        // 패턴이 쿨타임이다 -> 이동 or 일반 공격

        Vector3Int bossPos = controller.GridPos;
        Vector3Int playerPos = controller.TargetPos;
        int distance = GetDistanceTarget(bossPos, playerPos);

        if (controller.canPattern)
        {
            controller.canPattern = false;
            stateMachine.ChangeState(stateMachine.PatternState);
            return;
        }

        if (controller.isCooldown)
        {
            if (distance <= controller.maxAttackRange)
                stateMachine.ChangeState(stateMachine.AttackState);
            else
                stateMachine.ChangeState(stateMachine.MoveState);
            return;
        }

        bool isPlayerInRange = distance <= controller.patternRange;
        if (isPlayerInRange)
        {
            stateMachine.ChangeState(stateMachine.WarningState);
        }
        else
        {
            stateMachine.ChangeState(stateMachine.MoveState);
        }
    }

    public override void Exit()
    {
    }

    public override void Excute()
    {
    }
    
    
    private int GetDistanceTarget(Vector3Int pos, Vector3Int target)
    {
        return Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y);
    }

    public void DecidePreviewPath()
    {
        Vector3Int playerPos = controller.TargetPos;
        Vector3Int enemyPos = controller.GridPos;

        List<Vector3Int> path = GameManager.Map.FindPath(enemyPos, playerPos);

        if (path == null || path.Count == 0)
        {
            stateMachine.ChangeState(stateMachine.EndState);
            return;
        }

        if (path[path.Count - 1] == playerPos && GameManager.Map.IsPlayer(playerPos))
        {
            path.RemoveAt(path.Count - 1);
        }

        GameManager.Map.PlayerUpdateRange(controller.GridPos, controller.moveRange);

        controller.StartCoroutine(PreviewPathClear(path));
    }

    private IEnumerator PreviewPathClear(List<Vector3Int> path)
    {
        yield return new WaitForSeconds(1f);

        GameManager.Map.ClearPlayerRange();

        int distance = GetDistanceTarget(controller.GridPos, controller.TargetPos);

        if (distance >= controller.minAttackRange && distance <= controller.maxAttackRange)
        {
            stateMachine.ChangeState(stateMachine.AttackState);
        }
        else if (distance < controller.minAttackRange)
        {
            Vector3Int? retreat = SetRetreatPosition(controller.GridPos, controller.TargetPos, controller.moveRange);

            if (retreat.HasValue)
            {
                controller.TargetPos = retreat.Value;
                stateMachine.ChangeState(stateMachine.MoveState);
            }
        }
        else
        {
            stateMachine.ChangeState(stateMachine.MoveState);
        }
    }

    private Vector3Int? SetRetreatPosition(Vector3Int enemyPos, Vector3Int playerPos, int moveRange)
    {
        Vector3Int? bestCell = null;
        int bestDist = GetDistanceTarget(enemyPos, playerPos);

        for (int dx = -moveRange; dx <= moveRange; dx++)
        {
            for (int dy = -moveRange; dy <= moveRange; dy++)
            {
                Vector3Int candidate = new Vector3Int(enemyPos.x + dx, enemyPos.y + dy, 0);

                // �� ���̰� �̵� �����ؾ� ��
                if (!GameManager.Map.IsMovable(candidate))
                    continue;

                // �÷��̾���� �Ÿ�
                int dist = GetDistanceTarget(candidate, playerPos);

                if (dist > bestDist) // �� �ָ� �� �� �ִ� ĭ
                {
                    List<Vector3Int> path = GameManager.Map.FindPath(enemyPos, candidate);
                    if (path != null && path.Count <= moveRange)
                    {
                        bestDist = dist;
                        bestCell = candidate;
                    }
                }
            }
        }

        return bestCell;
    }
}
