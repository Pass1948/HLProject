using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemPatternState : PatternBossState
{
    public GolemPatternState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler) 
        : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        Debug.Log("GolemPattern boss state");
        // 패턴 공격 사용
        controller.StartCoroutine(RunPattern());
    }

    private IEnumerator RunPattern()
    {
        List<Vector3Int> attackCells = controller.warningCells;

        if (attackCells == null || attackCells.Count == 0)
        {
            Debug.Log("패턴 공격 불가");
            stateMachine.ChangeState(stateMachine.EndState);
            yield break;
        }

        GameManager.Map.attackRange.ClearRange();
        
        Transform player = GameManager.Unit.Player.transform;
        Vector3 dir = player.position - controller.transform.position;
        dir.y = 0f;

        controller.StartCoroutine(controller.SmoothRotate(dir, 0.12f));
        controller.animHandler.OnAttack(player);
        animHandler.OnPattern();
        yield return new WaitForSeconds(1f);

        foreach (Vector3Int cell in attackCells)
        {
            if (GameManager.Map.IsPlayer(cell))
            {
                GameManager.Unit.ChangeHealth(GameManager.Unit.Player.playerModel, controller.patternPower);
            }
        }

        yield return new WaitForSeconds(0.5f);
        controller.PatternCooldown();
        stateMachine.ChangeState(stateMachine.EndState);
    }
}
