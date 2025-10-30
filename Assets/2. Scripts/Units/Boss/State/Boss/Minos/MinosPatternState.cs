using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinosPatternState : PatternBossState
{
    // Start is called before the first frame update
    public MinosPatternState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }
    
    
    public override void Enter()
    {
        Debug.Log("MInosPattern boss state");
        // 패턴 공격 사용
        controller.StartCoroutine(RunPattern());
    }

    private IEnumerator RunPattern()
    {
        if (controller.warningCells == null || controller.warningCells.Count == 0)
        {
            yield break;
        }

        // 중심 라인(3x4 중 중앙) 계산
        Vector3Int bossPos = controller.GridPos;
        Vector3 forward = controller.transform.forward;
        Vector3Int dir = DirectionToGrid(forward);

        for (int i = 1; i <= 4; i++)
        {
            Vector3Int next = bossPos + dir * i;
            // 플레이어 충돌 체크
            if (GameManager.Map.IsPlayer(next))
            {
                GameManager.Unit.ChangeHealth(GameManager.Unit.Player.playerModel, controller.patternPower);
                // 사운드 추가
            }
            
            if (!GameManager.Map.IsMovable(next)) break;
            if (GameManager.Map.IsObstacle_Breakable(next))
            {
                // 여기서 플레이어 피해량 증가 시키기
            }
            
            Vector3 targetWorld = GameManager.Map.tilemap.GetCellCenterWorld(next);
            animHandler.OnPattern(controller.GridPos, controller.TargetPos);
            yield return controller.MoveToPosition(targetWorld, 0.08f);
        }

        controller.GridPos = GameManager.Map.grid.WorldToCell(controller.transform.position);
        controller.isCooldown = true;
        controller.remainCooldown = controller.cooldown;

        // 경고 타일 제거
        GameManager.Map.attackRange.ClearRange();

        stateMachine.ChangeState(stateMachine.EndState);
    }
    
    private Vector3Int DirectionToGrid(Vector3 dir)
    {
        dir.y = 0;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
            return (dir.x > 0) ? Vector3Int.right : Vector3Int.left;
        else
            return (dir.z > 0) ? Vector3Int.down : Vector3Int.up;
    }
}
