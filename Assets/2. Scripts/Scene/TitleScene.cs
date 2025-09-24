using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
    public override void SceneEnter()
    {
        GameManager.UI.OpenUI<TitleUI>();
    }
    
    public override void SceneLoading()
    {
        

    }
    
    public override void SceneExit()
    {

    }
}
