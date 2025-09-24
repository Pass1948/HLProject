using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    private Dictionary<int,int> stageRewards = new Dictionary<int,int>()
    {
        {1, 10},
        {2, 20},
        {3, 30},
        {4, 40},
        {5, 50},
    };
    

    public void GiveReward(int stageId)
    {
        if (stageRewards.TryGetValue(stageId, out int gold))
        {
            GameManager.Unit.Player.playerHandler.AddGold(gold);
        }
    }
}
