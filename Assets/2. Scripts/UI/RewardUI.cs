using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Analytics;

public class RewardUI : MonoBehaviour
{
    public RewardSlotUI[] slots;


    private void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].GetComponentInChildren<RewardSlotUI>();
        }
        // 디버깅 용 나중에 맵 스테이지 만들어지면 그때 제대로
        Show(1);
    }
    // 스테이지 인트 예시
    private readonly Dictionary<int,int> stageRewards = new()
    {
        {1, GameManager.TurnBased.turnSettingValue.rewardGold},
        {2, GameManager.TurnBased.turnSettingValue.rewardGold},
        {3, GameManager.TurnBased.turnSettingValue.rewardGold},
        {4, GameManager.TurnBased.turnSettingValue.rewardGold},
        {5, GameManager.TurnBased.turnSettingValue.rewardGold},
    };

    public void Show(int stageId)
    {
        int slotIndex = 0;

        if (stageRewards.TryGetValue(stageId, out int gold))
        {
            slots[slotIndex].gameObject.SetActive(true);
            slots[slotIndex].SetReward(gold);
            slotIndex++;
        }
        
        // 탄환 보상
        var bullets = GameManager.ItemControl.BulletWeightSampling(1);
        
        if(UnityEngine.Random.value < 0.1f)
            bullets.AddRange(GameManager.ItemControl.BulletWeightSampling(1));
        
        slots[0].gameObject.SetActive(true);
        
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



