using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
    public override void SceneEnter()
    {
        GameManager.UI.OpenUI<TitleUI>();
        GameManager.ItemControl.drawPile.Clear();
        GameManager.ItemControl.buyItems.Clear();
    }
    
    public override void SceneLoading()
    {
        

    }
    
    public override void SceneExit()
    {

    }
}
