using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : BaseTurnState
{
    public WinState() { }

    public void StageClearUI()
    {
        ResultUI backUI = GameManager.UI.GetUI<ResultUI>();
        backUI.GetResultType(ResultType.Clear);
        backUI.OpenUI();
    }

}
