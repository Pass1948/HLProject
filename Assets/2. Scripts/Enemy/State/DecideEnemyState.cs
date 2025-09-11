using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideEnemyState : BaseEnemyState
{
    public DecideEnemyState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Dictionary<IEnemyState, int> actionScores = new Dictionary<IEnemyState, int>();

        // ���� �ൿ�� ���� ������ �Űܼ� �������� -> �Ϲݸ��ʹ� �̵� / ����
        // ����Ʈ -> �̵� / ���� / + �ٸ���
        // ���� -> �̵� / ���� / + ���� ���ϵ�
    }

    public override void Excute()
    {
        Debug.Log("Decide : Excute");
    }

    public override void Exit()
    {
        Debug.Log("Decide : Exit");
    }
}
