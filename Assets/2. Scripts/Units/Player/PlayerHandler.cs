using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{

    private void Awake()
    {
    }
    public void Update()
    {

    }

    public void TakeDamage(int amount)
    {
        var player = GameManager.Unit.Player.playerModel;
        var vehicle = GameManager.Unit.Vehicle.vehicleModel;

        if (vehicle.condition == VehicleCondition.Riding && vehicle.health > 0)
        {
            GameManager.Unit.Vehicle.vehicleHandler.DamageVehicle(amount);
        }
        else
        {
            player.health -= amount;

            if (player.health <= 0)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
    public void VehicleControll()
    {

    }

}
