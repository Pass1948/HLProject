using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinState : BaseTurnState
{
    public WinState() { }

    public void StageClearUI()
    {
        //BaseUI backUI = GameManager.UI.GetUI<ResultUI>();
        //BaseUI clearUI = GameManager.UI.GetUI<ClearUI>();
        //clearUI.gameObject.transform.SetParent(backUI.transform);
    }

}
