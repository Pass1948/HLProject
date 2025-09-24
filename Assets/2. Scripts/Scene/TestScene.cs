using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTable;
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
        
        // GameManager.ItemControl.ItemDataSet();  // 아이템데이터 리스트 초기 세팅
    }

    public override void SceneExit()
    {
        
    }

    public override void SceneLoading()
    {
        
    }

}
