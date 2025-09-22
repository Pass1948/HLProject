using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    // 나중에 데이터 관리가 열리면 그 데이터로 연결.
    private int gold;

    private void Awake()
    {
        gold = 0;
    }

    public int GetGold()
    {
        return gold;
    }

    public void AddGold(int amount)
    {
        gold += amount;
        // 골드 추가시 여기에
        GameManager.Event.Publish(EventType.OnGoldChanged, gold);
        
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            GameManager.Event.Publish(EventType.OnGoldChanged, gold);
            return true;
        }
        else
        {
            Debug.Log("돈이 없으"); // TODO: 유아이 추가 작업 필요(JBS)
            return false;
        }
    }
}
