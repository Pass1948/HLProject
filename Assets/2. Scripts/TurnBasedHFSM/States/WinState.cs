using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : BaseTurnState
{
    public WinState() { }
    public override void OnEnter()
    {
        if (GameManager.Unit.boss!= null && GameManager.Unit.boss.model.isDie)
        {
            turnManager.ResetCount();
        }
        if(GameManager.Unit.boss == null)
        {
            StageClearUI();
            turnManager.ResetCount();
        }
    }
    public void StageClearUI()
    {
        ResultUI backUI = GameManager.UI.GetUI<ResultUI>();
        backUI.GetResultType(ResultType.Clear);
        backUI.OpenUI();
    }

}
