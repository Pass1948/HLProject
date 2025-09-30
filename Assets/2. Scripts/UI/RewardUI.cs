using System.Collections.Generic;
using UnityEngine;

public class RewardUI : MonoBehaviour
{
    public RewardSlotUI[] slots;
    
    private void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].GetComponentInChildren<RewardSlotUI>();
            slots[i].gameObject.SetActive(false);
        }
        Show(1);
    }
    // 스테이지 인트
    private Dictionary<int,int> stageRewards = new()
    {
        {1, 10},
        {2, 20},
        {3, 30},
        {4, 40},
        {5, 50},
    };

    public void Show(int stageId)
    {
        Debug.Log("Show Stage " + stageId);
        gameObject.SetActive(true);
        
        GiveReward(stageId);
        
        // 탄환 보상
        var bullets = GameManager.ItemControl.BulletWeightSampling(4);
        Debug.Log("불릿 개수 " + bullets.Count);
        Debug.Log("불릿 개수 " + slots.Length);
        int slotIndex = 0;
        
        if(UnityEngine.Random.value < 0.1f)
            bullets.AddRange(GameManager.ItemControl.BulletWeightSampling(1));
        
        foreach (var bullet in bullets)
        {
            if(slotIndex >= slots.Length) break;
            slots[slotIndex].gameObject.SetActive(true);
            slots[slotIndex].SetReward(bullet);
            slotIndex++;
        }
        
        // 특수유물은 어떻게 해
        if (BossStageCheck(stageId))
        {
            var relicCandirates = GameManager.ItemControl.RelicWeightSampling(1); 
            // 플레이어가 보유한 유물 제외
            relicCandirates.RemoveAll(r => GameManager.ItemControl.buyItems.Exists(b => b.id == r.id));
            
            if(slotIndex >= slots.Length) return;
            slots[slotIndex].gameObject.SetActive(true);
            slots[slotIndex].SetReward(relicCandirates[0]);
            slotIndex++;
        }
        for(; slotIndex < slots.Length; slotIndex++)
            slots[slotIndex].gameObject.SetActive(false);
    }

    private bool BossStageCheck(int stageId)
    {
        return (stageId % 4 == 0);
    }
    private void GiveReward(int stageId)
    {
        if (stageRewards.TryGetValue(stageId, out int gold))
        {
            GameManager.Unit.Player.playerHandler.AddGold(gold);
            
        }
    }
}



