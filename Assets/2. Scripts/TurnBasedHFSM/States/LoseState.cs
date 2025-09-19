using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseState : BaseTurnState
{
   public LoseState() { }

    public override void OnEnter()
    {
        GameOverUI();
    }
    public void GameOverUI()
    {
        ResultUI backUI = GameManager.UI.GetUI<ResultUI>();
        backUI.GetResultType(ResultType.Over);
        backUI.OpenUI();
    }
}
