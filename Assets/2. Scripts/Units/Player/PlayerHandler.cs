using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerHandler : MonoBehaviour
{
    public int playerMonney;
    
    public List<Ammo> bullets = new();
    public List<Ammo> GetBullet() { return bullets;}
    
    public List<int> ownedRelics = new(); // 유물 리스트
 
    private void Start()
    {
        playerMonney = 500000000;
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
            if (player == null) return;

            player.currentHealth = Mathf.Max(0, player.currentHealth - amount);

            if (player.currentHealth <= 0)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
    // 회복.
    public void Heal(int amount)
    {
        GameManager.Unit.Player.playerModel.currentHealth += amount;
        if (GameManager.Unit.Player.playerModel.currentHealth >= GameManager.Unit.Player.playerModel.maxHealth)
        {
            GameManager.Unit.Player.playerModel.currentHealth = GameManager.Unit.Player.playerModel.maxHealth;
        }
        Debug.Log($"체력 회복: {amount}, 현재 체력: {GameManager.Unit.Player.playerModel.currentHealth}");
    }
    

    public int GetGold()
    {
        return playerMonney;
    }

    public void AddGold(int amount)
    {
        playerMonney += amount;
        // 골드 추가시 여기에
    }

    public bool SpendGold(int amount)
    {
        if (playerMonney >= amount)
        {
            playerMonney -= amount;
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
