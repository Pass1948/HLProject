using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieBossState : BaseBossState
{
    public DieBossState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler)
        : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        if (controller.isDie)
        {
            animHandler.OnDie();
            GameManager.Sound.PlayBossDeathSound();
            Exit();
        }
    }

    public override void Exit()
    {
        GameManager.Map.SetObjectPosition(
            controller.GridPos.x, 
            controller.GridPos.y, 
            TileID.Terrain
        );
        // GameManager.Unit.enemies.Remove(controller.baseBoss);
        controller.baseBoss.gameObject.SetActive(false);
        //controller.baseEnemy.gameObject.Destroy();
        if (GameManager.TurnBased.turnSettingValue.isTutorial) return;
        StageClearUI();
    }

    public override void Excute()
    {
        throw new System.NotImplementedException();
    }

    public void StageClearUI()
    {
        ResultUI backUI = GameManager.UI.GetUI<ResultUI>();
        backUI.GetResultType(ResultType.GameClear);
        backUI.OpenUI();
    }
}
