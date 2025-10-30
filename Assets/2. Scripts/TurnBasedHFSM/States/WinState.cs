using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : BaseTurnState
{
    public WinState() { }

    float timer;
    private bool didClose;
    public override void OnEnter()
    {
            timer = turnSetVlaue.resetTime;
    }
    public override void Tick(float dt)
    {
        // 이미 처리했다면 더 이상 검사하지 않음
        if (didClose) return;

        timer += dt;
        if (timer > turnSetVlaue.turnDelayTime)
        {
            if (GameManager.Unit.boss != null && GameManager.Unit.boss.model.isDie)
            {
                turnManager.ResetCount();
                didClose = true;
            }
            if (GameManager.Unit.boss == null)
            {
                StageClearUI();
                turnManager.ResetCount();
                didClose = true;
            }
        }
    }
    public void StageClearUI()
    {
        ResultUI backUI = GameManager.UI.GetUI<ResultUI>();
        backUI.GetResultType(ResultType.Clear);
        backUI.OpenUI();
    }

}
