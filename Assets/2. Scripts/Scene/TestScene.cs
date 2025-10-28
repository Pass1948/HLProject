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
        // Debug.Log("TestScene : SceneEnter");
        _stage = new Stage();
        // Debug.Log("new Stage");
        // Debug.Log(GameManager.SaveLoad.nextSceneIndex);
        _stage.InitStage(GameManager.Stage.stageId);
        GameManager.UI.OpenUI<FadeOutUI>();
        GameManager.Map.CreateMap(_stage);
        var cam = GameManager.Resource.Create<GameObject>(Path.Camera + "MainCamera");
        CameraController cc = cam.GetComponent<CameraController>();
        cc.InitCamera();
        if(GameManager.Stage.stageId >= 7002)
        {
            GameManager.Unit.Vehicle.vehicleHandler.MountVehicle();
        }
        GameManager.TurnBased.ChangeStartTurn();
        GameManager.ItemControl.ItemDataSet();  // 아이템데이터 리스트 초기 세팅
        GameManager.Mouse.CreateMouse();
        GameManager.Shop.ShopInit(_stage);
        // Sound
        var gameBgm = GameManager.Resource.Load<AudioClip>(Path.Sound + "NeonCityPaladin");
        GameManager.Sound.PlayBGM(gameBgm);
        
        if (GameManager.Unit.isInit == true)
        {
            GameManager.Unit.SetCurrentStat();
        }
    }

    public override void SceneExit()
    {
        _stage = null;
        GameManager.Unit.AllClearEnemies();
        GameManager.Unit.enemies.Clear();
        GameManager.Mouse.ClearMouse();
    }

    public override void SceneLoading()
    {
        
    }
}
