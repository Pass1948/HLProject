using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    private BasePlayer player;
    private BaseVehicle vehicle;

    private bool playerBording => player.playerModel.viecleBording == ViecleBording.On;

    private void Awake()
    {
        player = GameManager.Unit.Player;
        vehicle = GameManager.Unit.Vehicle;
    }
    public void Update()
    {
        
    }
    //데미지 처리
    public void TakeDamage(int amount)
    {
        // 먼저 오토바이에 들어갑니다
        vehicle.vehicleHandler.DamageVehicle(amount);

        if(vehicle.vehicleModel.health <= 0 && !playerBording)
        {
            player.playerModel.health -= amount;
        }
        if(player.playerModel.health <= 0)
        {
            // 죽는 UI
        }
        else
        {
            vehicle.vehicleModel.health -= amount;
        }
    }
    public void VehicleControll()
    {

    }

}
