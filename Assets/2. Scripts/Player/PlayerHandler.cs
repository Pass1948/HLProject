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

    public void TakeDamage(int amount)
    {
        vehicle.vehicleHandler.DamageVehicle(amount);
        if(vehicle.vehicleModel.health <= 0 && playerBording)

        {
            player.playerModel.health -= amount;
        }
        if(player.playerModel.health <= 0)
        {

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
