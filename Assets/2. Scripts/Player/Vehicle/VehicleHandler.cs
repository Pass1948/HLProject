using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class VehicleHandler : MonoBehaviour
{
   
    [SerializeField] private int repairAmount = 3;

    [SerializeField] private GameObject vehicleDestruction; // 부셔지면 띄우는 이미지

    public bool isMounted => GameManager.Unit.Player.playerModel.viecleBording == ViecleBording.On;

    private int currentPlayerMoveRange;
    private int currentPlayerHP;

    private void Awake()
    {
        GameManager.Unit.Player.playerModel.viecleBording = ViecleBording.On;
        
        MountVehicle();
        currentPlayerMoveRange = GameManager.Unit.Player.playerModel.moveRange;
        currentPlayerHP = GameManager.Unit.Player.playerModel.health;
    }
    private void Start()
    {
        GetEffectiveMoveRange();
        vehicleDestruction = GameManager.Resource.Create<GameObject>(Path.Player + "VehicleDistructionImage", transform);
        vehicleDestruction.SetActive(false);
    }


    // Debugging Dragon
    public void OnClickTestBtn()
    {
        DamageVehicle(3);
    }

    // 플레이어와 오토바의 이동범위 합치는 로직
    public int GetEffectiveMoveRange()
    {
        if (isMounted && GameManager.Unit.Vehicle.vehicleModel.health > 0)
        {
            return GameManager.Unit.Player.playerModel.moveRange + GameManager.Unit.Vehicle.vehicleModel.additinalMove;
        }
        else
            return GameManager.Unit.Player.playerModel.moveRange;
    }

    // 데미지 받는 로직
    public void DamageVehicle(int amount )
    {
        GameManager.Unit.Vehicle.vehicleModel.health -= amount;
        vehicleDestruction.transform.position = transform.position;
        if(GameManager.Unit.Vehicle.vehicleModel.health <= 0)
        {
            GameManager.Unit.Vehicle.vehicleModel.condition = VehicleCondition.Destruction;
            GameManager.Unit.Player.playerModel.viecleBording = ViecleBording.off;
            GameManager.Unit.Vehicle.transform.SetParent(null);
            vehicleDestruction.SetActive(true); 
            GameManager.Map.mapData[(int)transform.position.x, (int)transform.position.y] = TileID.Vehicle;
            GameManager.Unit.Player.playerModel.moveRange = currentPlayerMoveRange;
            GameManager.Unit.Player.playerModel.health -= GameManager.Unit.Vehicle.vehicleModel.health;
        }
        else
        {
            vehicleDestruction.SetActive(false); 
        }
    }

    // 오토바이 고치는 로직
    public void RepairVehicle()
    {
        GameManager.Unit.Vehicle.vehicleModel.health += repairAmount;
        if(GameManager.Unit.Vehicle.vehicleModel.health > 0)
        {
            GameManager.Unit.Vehicle.vehicleModel.condition = VehicleCondition.Riding;
            MountVehicle();
        }
    }


    //탑승 버튼
    public void MountVehicle()
    {
        GameManager.Unit.Vehicle.vehicleModel.condition = VehicleCondition.Riding;
        GameManager.Unit.Player.playerModel.viecleBording = ViecleBording.On;
        transform.SetParent(GameManager.Unit.Player.transform);
        GameManager.Unit.Vehicle.transform.localPosition = Vector3.zero;
        GameManager.Unit.Player.playerModel.moveRange += GameManager.Unit.Vehicle.vehicleModel.moveRange;
        GameManager.Unit.Player.playerModel.health += GameManager.Unit.Vehicle.vehicleModel.health;
        GameManager.Map.mapData[(int)transform.position.x, (int)transform.position.y] = 0;
    }
    // 내리는 버튼
    public void DismountVehicle()
    {
        GameManager.Unit.Vehicle.vehicleModel.condition = VehicleCondition.GetOff;
        GameManager.Unit.Player.playerModel.viecleBording = ViecleBording.off;
        GameManager.Unit.Vehicle.transform.SetParent(null);
        GameManager.Unit.Player.playerModel.moveRange = currentPlayerMoveRange;
        GameManager.Unit.Player.playerModel.health -= GameManager.Unit.Vehicle.vehicleModel.health;
        GameManager.Map.mapData[(int)transform.position.x, (int)transform.position.y] = TileID.Vehicle;
    }
    
}
