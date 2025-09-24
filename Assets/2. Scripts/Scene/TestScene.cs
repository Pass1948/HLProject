using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestScene : BaseScene
{
    public override void SceneEnter()
    {
        GameManager.Data.Initialize();
        GameManager.Map.CreateMap();
        GameManager.Mouse.CreateMouse();
        GameManager.Unit.Vehicle.vehicleHandler.MountVehicle();
    }

    public override void SceneExit()
    {
        
    }

    public override void SceneLoading()
    {
        
    }

}
