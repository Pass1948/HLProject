using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{

    public BasePlayer player;
    public BaseVehicle vehicle;

    private bool playerBording => player.playerModel.viecleBording == ViecleBording.On;

    private void Awake()
    {
        player = new BasePlayer();
        vehicle = new BaseVehicle();
    }

    public void TakeDamage(int amount)
    {
        if(vehicle.vehicleModel.health <= 0 && playerBording)
        {
            player.playerModel.health -= amount;
        }
        if(player.playerModel.health <= 0)
        {
            Debug.Log("게임오바야 오바");
        }
        else
        {
            vehicle.vehicleModel.health -= amount;
        }
    }

}
