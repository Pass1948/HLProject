using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : BaseTurnState
{
    ResultUI backUI = GameManager.UI.GetUI<ResultUI>();
    public WinState() { }
    public override void OnEnter()
    {
        StageClearUI();
        turnManager.ResetCount();
        
    }
    public void StageClearUI()
    {
        backUI.GetResultType(ResultType.Clear);
        backUI.OpenUI();
    }

}
