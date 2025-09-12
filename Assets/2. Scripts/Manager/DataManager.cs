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
    // hp �����
    public void ChangeHp(float value)
    {
        playerData.playerMoveData.HP += value;
        playerData.playerMoveData.HP = Mathf.Clamp(playerData.playerMoveData.HP, 0, 10);
    }

    // �����̻� �߰���(�� �߰� �� �ؿ� ���� ó��)
    public void ApplyPoison(int turn)
    {
        playerData.playerConditionData.PoisonTurn = turn;
    }
    public void ApplySturn(int turn)
    {
        playerData.playerConditionData.StunTurn = turn;
    }
    // ���࿡ �������� ���� �Ϸ���
    // playerData.playerAttackData.AttackDamage += value; �̷� ������ �ϸ� �˴ϴ���


    // ���Ⱑ �ƴ� �ٸ� cs���Ͽ�����
    // GameManager.Data.�Լ���() ���� �����ϸ� �˴ϴ���

}
