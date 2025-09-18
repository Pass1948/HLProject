using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHandler : MonoBehaviour
{
    // TODO: ������ �ѹ��� �����ϰ� ��. ���߿� RepairVehicle() �Լ��� �Ű������� ������ �ް� �� ������(JBS)
    [SerializeField] private int repairAmount = 3; // ������

    private BasePlayer player;
    private BaseVehicle vehicle;
    public bool isMounted => GameManager.Unit.Player.playerModel.viecleBording == ViecleBording.On; // ž�� ����

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


    }
    private void OnDisable()
    {

    }


    // Debugging Dragon
    public void OnClickTestBtn()
    {
        DamageVehicle(3);
    }

    // ž�� ���� �̸鼭 ü���� 0�� �ƴϸ� �̵��� ����
    public int GetEffectiveMoveRange()
    {
        if (isMounted && vehicle.vehicleModel.health > 0)
        {
            Debug.Log("����ũ ž�»���");
            return GameManager.Unit.Player.playerModel.moveRange + GameManager.Unit.Vehicle.vehicleModel.additinalMove;
        }
        else
            return GameManager.Unit.Player.playerModel.moveRange;
    }

    // ž�� ����, ü���� 0�� �ƴϸ� ��� ������ ���� 0 �Ǹ� �ı�
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

    // ž������ ����,���� ���,�����ϸ� ž�� ���·� ����
    //TODO: ���࿡ ������ �� �� ���� ��� �� ��� ������ ���� ����.(�庸��)
    public void RepairVehicle()
    {
        GameManager.Unit.Vehicle.vehicleModel.health += repairAmount;
        if(vehicle.vehicleModel.health > 0)
        {
            MountVehicle();
        }
    }
    // ž�� ����
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
    // ž�� ����
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
