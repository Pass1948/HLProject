using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerHandler : MonoBehaviour
{
    public int playerMonney;
    
    public List<Ammo> bullets = new();
    public List<Ammo> GetBullet() { return bullets;}
    
    public List<int> ownedRelics = new();
    
    private void Awake()
    {
        playerMonney = 0;
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
    // 회복.
    public void Heal(int amount)
    {
        var player = GameManager.Unit.Player.playerModel;
        player.health += amount;
        Debug.Log($"체력 회복: {amount}, 현재 체력: {player.health}");
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
    //탄환 관리
    public void AddBullet(Ammo ammo)
    {
        if(ammo != null)
            bullets.Add(ammo);
    }

    public void RemoveBullet(Ammo ammo)
    {
        if(bullets.Contains(ammo))
            bullets.Remove(ammo);
    }

    public void AddRelic(int relicid)
    {
        ownedRelics.Add(relicid);
    }
    
    
    
}
