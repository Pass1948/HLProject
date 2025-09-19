using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{

    private BasePlayer player;
    private BaseVehicle vehicle;

    private bool playerBording => GameManager.Unit.Player.playerModel.viecleBording == ViecleBording.On;

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
        GameManager.Unit.Vehicle.vehicleHandler.DamageVehicle(amount);
        if(GameManager.Unit.Vehicle.vehicleModel.health <= 0 && playerBording)
        {
            GameManager.Unit.Player.playerModel.health -= amount;
            Debug.Log($"플레이어 체력 : {GameManager.Unit.Player.playerModel.health}");
        }
        if(GameManager.Unit.Player.playerModel.health <= 0)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            GameManager.Unit.Vehicle.vehicleModel.health -= amount;
            Debug.Log($"오토바이 체력 : {GameManager.Unit.Player.playerModel.health}");
        }
    }
    public void VehicleControll()
    {

    }

}
