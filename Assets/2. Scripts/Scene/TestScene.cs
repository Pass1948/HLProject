using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TestScene : BaseScene
{
    public override void SceneEnter()
    {
        var ui = GameManager.UI.GetUI<TestUI>();
        ui.OpenUI();


    }

    public override void SceneExit()
    {
        
    }

    public override void SceneLoading()
    {
        
    }

}
