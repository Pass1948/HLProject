using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseState : BaseTurnState
{        

   public LoseState() { }
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
            GameOverUI();
            turnManager.ResetCount();
        }
    }
    public void GameOverUI()
    {
        ResultUI backUI = GameManager.UI.GetUI<ResultUI>();
        backUI.GetResultType(ResultType.Over);
        backUI.OpenUI();
    }
}
