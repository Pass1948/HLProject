using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHandler : MonoBehaviour
{
    // TODO: 아직은 한번에 수리하게 함. 나중에 RepairVehicle() 함수에 매개변수로 수리량 받게 할 수도잇(JBS)
    [SerializeField] private int repairAmount = 3; // 수리량

    public Transform player;
    public Transform vehicle;

    public PlayerModel playerModel;
    public VehicleModel vehicleModel;

    public bool isMounted => playerModel.viecleBording == ViecleBording.On; // 탑승 상태


    private void Awake()
    {
        playerModel = GameManager.Unit.Player.playerModel;
        vehicleModel = GameManager.Unit.Vehicle.vehicleModel;

        playerModel.viecleBording.CompareTo(ViecleBording.On);

    }
    // 탑승 상태,체력이 0이 아니면 이동력 증가
    public int GetEffectiveMoveRange()
    {
        if(isMounted && vehicleModel.health > 0)
            return  playerModel.moveRange + vehicleModel.additinalMove;
        else
            return playerModel.moveRange;
    }
    // 탑승 상태, 체력이 0이 아니면 대신 데미지 받음 0 되면 파괴
    public void DamageVehicle(int amount)
    {
        vehicleModel.health -= amount;

        if(vehicleModel.health <= 0)
        {
            vehicleModel.condition = VehicleCondition.Destruction;
        }
    }
    // 탑승해제 상태,수리하면 탑승 상태로 변경
    public void RepairVehicle()
    {
        vehicleModel.health += repairAmount;
        if(vehicleModel.health > 0)
        {
            vehicleModel.condition = VehicleCondition.Bording;
        }
        MountVehicle();
    }
    // 탑승
    public void MountVehicle()
    {
        if(vehicleModel.isDestruction)
        {
            return;
        }
        playerModel.viecleBording = ViecleBording.On;
        vehicle.SetParent(player);
        vehicle.localPosition = Vector3.zero;
    }
    // 탑승 해제
    public void DismountVehicle()
    {
        playerModel.viecleBording = ViecleBording.off;
        vehicle.SetParent(null);
    }
}
