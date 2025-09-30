using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataTable;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestScene : BaseScene
{
    private Stage _stage;

    public override void SceneEnter()
    {
        Debug.Log("스테이지 씬 시작");
        _stage = new Stage();
        _stage.InitStage();
        GameManager.Map.CreateMap(_stage);
        var cam = GameManager.Resource.Create<GameObject>(Path.Camera + "MainCamera");
        CameraController cc = cam.GetComponent<CameraController>();
        cc.InitCamera();
        GameManager.Mouse.CreateMouse();
        GameManager.Unit.Vehicle.vehicleHandler.MountVehicle();

        GameManager.TurnBased.ChangeStartTurn();
        GameManager.ItemControl.ItemDataSet();  // 아이템데이터 리스트 초기 세팅
        GameManager.Shop.ShopInit();
    }

    public override void SceneExit()
    {
        
    }

    public override void SceneLoading()
    {
        
    }
}
