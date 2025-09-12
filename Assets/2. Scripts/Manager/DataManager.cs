using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private PlayerData playerData;
    private AutoCicleData autoCicleData;
    private void Awake()
    {
        playerData = new PlayerData();
        autoCicleData = new AutoCicleData();
    }
    // hp 변경시
    public void ChangeHp(float value)
    {
        playerData.playerMoveData.HP += value;
        playerData.playerMoveData.HP = Mathf.Clamp(playerData.playerMoveData.HP, 0, 10);
    }

    // 상태이상 추가시(더 추가 시 밑에 예시 처럼)
    public void ApplyPoison(int turn)
    {
        playerData.playerConditionData.PoisonTurn = turn;
    }
    public void ApplySturn(int turn)
    {
        playerData.playerConditionData.StunTurn = turn;
    }
    // 만약에 데미지를 변경 하려면
    // playerData.playerAttackData.AttackDamage += value; 이런 식으로 하면 됩니다잇


    // 여기가 아닌 다른 cs파일에서는
    // GameManager.Data.함수명() 으로 접근하면 됩니다잇

}
