using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHandler : MonoBehaviour
{
    // TODO: 아직은 한번에 수리하게 함. 나중에 RepairVehicle() 함수에 매개변수로 수리량 받게 할 수도잇(JBS)
    [SerializeField] private int repairAmount = 3; // 수리량

    private BasePlayer player;
    private BaseVehicle vehicle;
    public bool isMounted => player.playerModel.viecleBording == ViecleBording.On; // 탑승 상태

    private void OnEnable()
    {
        GameManager.Event.Subscribe(EventType.VehicleOnRepaired, RepairVehicle);
        GameManager.Event.Subscribe(EventType.VehicleOnRepaired, RepairVehicle);

    }
    private void OnDisable()
    {
        GameManager.Event.Unsubscribe(EventType.VehicleOnRepaired, RepairVehicle);
    }

    private void Awake()
    {
        player = GameManager.Unit.Player;
        vehicle = GameManager.Unit.Vehicle;

        player.playerModel.viecleBording.CompareTo(ViecleBording.On);

        MountVehicle();
    }
    private void Start()
    {
        GetEffectiveMoveRange();
    }

    // Debugging Dragon
    public void OnClickTestBtn()
    {
        DamageVehicle(3);
    }

    // 탑승 상태 이면서 체력이 0이 아니면 이동력 증가
    public int GetEffectiveMoveRange()
    {
        if (isMounted && vehicle.vehicleModel.health > 0)
        {
            Debug.Log("바이크 탑승상태");
            return player.playerModel.moveRange + vehicle.vehicleModel.additinalMove;
        }
        else
            return player.playerModel.moveRange;
    }

    // 탑승 상태, 체력이 0이 아니면 대신 데미지 받음 0 되면 파괴
    public void DamageVehicle(int amount)
    {
        vehicle.vehicleModel.health -= amount;

        if(vehicle.vehicleModel.health <= 0)
        {
            vehicle.vehicleModel.condition = VehicleCondition.Destruction;
            DismountVehicle();
        }
    }

    // 탑승해제 상태,수리하면 탑승 상태로 변경
    public void RepairVehicle()
    {
        vehicle.vehicleModel.health += repairAmount;
        if(vehicle.vehicleModel.health > 0)
        {
            vehicle.vehicleModel.condition = VehicleCondition.Bording;
        }
        MountVehicle();
    }

    // 탑승 히잇
    public void MountVehicle()
    {
        if(vehicle.vehicleModel.isDestruction)
        {
            return;
        }
        player.playerModel.viecleBording = ViecleBording.On;
        transform.SetParent(player.transform);
        vehicle.transform.localPosition = Vector3.zero;
        player.playerModel.moveRange += vehicle.vehicleModel.moveRange;
        player.playerModel.health += vehicle.vehicleModel.health;
    }
    // 탑승 해제
    public void DismountVehicle()
    {
        player.playerModel.viecleBording = ViecleBording.off;
        vehicle.transform.SetParent(null);
        player.playerModel.moveRange -= vehicle.vehicleModel.moveRange;
        player.playerModel.health -= vehicle.vehicleModel.health;
    }
    
}
