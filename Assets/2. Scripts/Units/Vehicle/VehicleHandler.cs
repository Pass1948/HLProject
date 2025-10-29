using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHandler : MonoBehaviour
{

    [SerializeField] private int repairAmount = 3;

    [SerializeField] public GameObject vehicleDestruction; // 부셔지면 띄우는 이미지

    public bool isMounted => GameManager.Unit.Player.playerModel.viecleBording == ViecleBording.On;

    private int currentPlayerMoveRange;
    private int currentPlayerHP;

    public Vector3Int vehiclePoison;


    private void Awake()
    {
        GameManager.Unit.Player.playerModel.viecleBording = ViecleBording.On;

        GameManager.Unit.Vehicle.vehicleModel.condition = VehicleCondition.GetOff;
        currentPlayerMoveRange = 1;
        currentPlayerHP = GameManager.Unit.Player.playerModel.currentHealth;
    }
    private void Start()
    {
        SetPosition();
        GetEffectiveMoveRange();
        vehicleDestruction = GameManager.Resource.Create<GameObject>(Path.Player + "VehicleDistructionImage", transform);
        vehicleDestruction.SetActive(false);
        this.transform.forward = GameManager.Unit.Player.controller.transform.forward;
    }

    public void SetPosition()
    {
        vehiclePoison = new Vector3Int((int)transform.position.x, (int)transform.position.z, 0);
    }

    public void OnPositionForward()
    {
        this.transform.forward = GameManager.Unit.Player.controller.transform.forward;
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
    public void DamageVehicle(int amount)
    {
        GameManager.Unit.Vehicle.vehicleModel.currentHealth = Mathf.Max(0, GameManager.Unit.Vehicle.vehicleModel.currentHealth - amount);
        vehicleDestruction.transform.position = transform.position;
        if (GameManager.Unit.Vehicle.vehicleModel.currentHealth <= 0)
        {
            GameManager.Unit.Vehicle.vehicleModel.condition = VehicleCondition.Destruction;
            GameManager.Unit.Player.playerModel.viecleBording = ViecleBording.off;
            GameManager.Unit.Vehicle.transform.SetParent(null);
            vehicleDestruction.SetActive(true);
            GameManager.Map.mapData[(int)transform.position.x, (int)transform.position.y] = TileID.Vehicle;
            GameManager.Unit.Player.playerModel.moveRange = currentPlayerMoveRange;
            // GameManager.Unit.Player.playerModel.health -= GameManager.Unit.Vehicle.vehicleModel.health;
            GameManager.Unit.Player.animHandler.OnRiding();
        }
        else
        {
            vehicleDestruction.SetActive(false);
        }
    }

    // 오토바이 고치는 로직
    public void RepairVehicle()
    {

        GameManager.Unit.Vehicle.vehicleModel.currentHealth += repairAmount;
        if (GameManager.Unit.Vehicle.vehicleModel.currentHealth > 0)
        {
            GameManager.Unit.Vehicle.vehicleModel.condition = VehicleCondition.Riding;
            MountVehicle();
            GameManager.Unit.Player.animHandler.OnRiding();
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
        //GameManager.Unit.Player.playerModel.health += GameManager.Unit.Vehicle.vehicleModel.health;
        GameManager.Map.mapData[(int)transform.position.x, (int)transform.position.y] = 0;
        GameManager.Unit.Player.animHandler.OnRiding();
    }
    // 내리는 버튼
    public void DismountVehicle()
    {
        GameManager.Unit.Vehicle.vehicleModel.condition = VehicleCondition.GetOff;
        GameManager.Unit.Player.playerModel.viecleBording = ViecleBording.off;
        GameManager.Unit.Vehicle.transform.SetParent(null);
        GameManager.Unit.Player.playerModel.moveRange = currentPlayerMoveRange;
        // GameManager.Unit.Player.playerModel.health -= GameManager.Unit.Vehicle.vehicleModel.health;
        GameManager.Map.mapData[(int)transform.position.x, (int)transform.position.y] = TileID.Vehicle;
        GameManager.Unit.Player.animHandler.OnRiding();
    }

}
