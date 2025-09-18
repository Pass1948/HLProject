using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHandler : MonoBehaviour
{
    // TODO: 아직은 한번에 수리하게 함. 나중에 RepairVehicle() 함수에 매개변수로 수리량 받게 할 수도잇(JBS)
    [SerializeField] private int repairAmount = 3; // 수리량

    private BasePlayer player;
    private BaseVehicle vehicle;
    public bool isMounted => GameManager.Unit.Player.playerModel.viecleBording == ViecleBording.On; // 탑승 상태

    private void Awake()
    {
        player = GameManager.Unit.Player;
        vehicle = GameManager.Unit.Vehicle;

        GameManager.Unit.Player.playerModel.viecleBording.CompareTo(ViecleBording.On);

        MountVehicle();
    }
    private void Start()
    {
        GetEffectiveMoveRange();
    }
    private void OnEnable()
    {
        //GameManager.Event.Subscribe(EventType.VehicleOnRepaired, RepairVehicle);

    }
    private void OnDisable()
    {
        //GameManager.Event.Unsubscribe(EventType.VehicleOnRepaired, RepairVehicle);
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
            return GameManager.Unit.Player.playerModel.moveRange + GameManager.Unit.Vehicle.vehicleModel.additinalMove;
        }
        else
            return GameManager.Unit.Player.playerModel.moveRange;
    }

    // 탑승 상태, 체력이 0이 아니면 대신 데미지 받음 0 되면 파괴
    public void DamageVehicle(int amount )
    {
        GameManager.Unit.Vehicle.vehicleModel.health -= amount;

        if(vehicle.vehicleModel.health <= 0)
        {
            GameManager.Unit.Vehicle.vehicleModel.condition = VehicleCondition.Destruction;
            GameManager.Unit.Player.playerModel.viecleBording.CompareTo(ViecleBording.off);
            DismountVehicle();
        }
    }

    // 탑승해제 상태,수리 모드,수리하면 탑승 상태로 변경
    //TODO: 만약에 수리를 할 때 턴을 써야 할 경우 변수로 값을 변경.(장보석)
    public void RepairVehicle()
    {
        GameManager.Unit.Vehicle.vehicleModel.health += repairAmount;
        if(vehicle.vehicleModel.health > 0)
        {
            MountVehicle();
        }
    }
    // 탑승 히잇
    public void MountVehicle()
    {
        if(GameManager.Unit.Vehicle.vehicleModel.isDestruction)
        {
            return;
        }
        GameManager.Unit.Vehicle.vehicleModel.condition = VehicleCondition.Riding;
        GameManager.Unit.Player.playerModel.viecleBording = ViecleBording.On;
        transform.SetParent(player.transform);
        GameManager.Unit.Vehicle.transform.localPosition = Vector3.zero;
        GameManager.Unit.Player.playerModel.moveRange += GameManager.Unit.Vehicle.vehicleModel.moveRange;
        GameManager.Unit.Player.playerModel.health += GameManager.Unit.Vehicle.vehicleModel.health;
    }
    // 탑승 해제
    public void DismountVehicle()
    {
        GameManager.Unit.Vehicle.vehicleModel.condition = VehicleCondition.GetOff;
        GameManager.Unit.Player.playerModel.viecleBording = ViecleBording.off;
        GameManager.Unit.Vehicle.transform.SetParent(null);
        if(vehicle.vehicleModel.health > 0)
        {
            GameManager.Unit.Player.playerModel.moveRange -= GameManager.Unit.Vehicle.vehicleModel.moveRange;
            GameManager.Unit.Player.playerModel.health -= GameManager.Unit.Vehicle.vehicleModel.health;
        }
    }
    
}
