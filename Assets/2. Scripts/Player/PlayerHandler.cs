using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public int playerMonney;
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
    private void Awake()
    {
        playerMonney = 0;
    }

    public int GetGold()
    {
        return playerMonney;
    }

    public void AddGold(int amount)
    {
        playerMonney += amount;
        // 골드 추가시 여기에
        GameManager.Event.Publish(EventType.OnGoldChanged, playerMonney);
        
    }

    public bool SpendGold(int amount)
    {
        if (playerMonney >= amount)
        {
            playerMonney -= amount;
            GameManager.Event.Publish(EventType.OnGoldChanged, playerMonney);
            return true;
        }
        else
        {
            Debug.Log("돈이 없으"); // TODO: 유아이 추가 작업 필요(JBS)
            return false;
        }
    }
    
    
    
    
}
