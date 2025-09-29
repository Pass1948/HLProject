using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseState : BaseTurnState
{        
    ResultUI backUI = GameManager.UI.GetUI<ResultUI>();
   public LoseState() { }

    public override void OnEnter()
    {
        GameOverUI();
        turnManager.ResetCount();
    }
    public void GameOverUI()
    {

        backUI.GetResultType(ResultType.Over);
        backUI.OpenUI();
    }
}
