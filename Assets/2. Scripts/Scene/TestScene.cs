using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestScene : BaseScene
{
    public override void SceneEnter()
    {
        GameManager.Map.CreateMap();
        GameManager.Mouse.CreateMouse();
    }

    public override void SceneExit()
    {
        
    }

    public override void SceneLoading()
    {
        
    }

}
