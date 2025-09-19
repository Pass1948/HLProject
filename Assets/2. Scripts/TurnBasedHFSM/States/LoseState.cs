using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseState : BaseTurnState
{
   public LoseState() { }

    public void GameOverUI()
    {
        //BaseUI backUI = GameManager.UI.GetUI<ResultUI>();
        //BaseUI overUI = GameManager.UI.GetUI<OverUI>();
        //overUI.gameObject.transform.SetParent(backUI.transform);
    }
}
