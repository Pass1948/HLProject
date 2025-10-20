using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemPatternState : PatternBossState
{
    public GolemPatternState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler) 
        : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
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
        animHandler.OnAttack();
        yield return new WaitForSeconds(1f);

        foreach (Vector3Int cell in attackCells)
        {
            if (GameManager.Map.IsPlayer(cell))
            {
                GameManager.Unit.ChangeHealth(GameManager.Unit.Player.playerModel, controller.model.attack);
            }
        }

        yield return new WaitForSeconds(0.5f);
        
        stateMachine.ChangeState(stateMachine.EndState);
    }
}
