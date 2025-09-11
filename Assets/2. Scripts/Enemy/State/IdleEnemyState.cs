using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� ���� ����ϰ� �ִ� ����
public class IdleEnemyState : BaseEnemyState
{
    public IdleEnemyState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Idle : Enter");

    }

    public override void Excute()
    {
        Debug.Log("Idle : Excute");

        // �����
        // do nothing!!
        // �� ������ ��� Evaluate ��ȯ
        // or
        // Hit �� ��� Hit���� ��ȯ
    }

    public override void Exit()
    {
        Debug.Log("Idle : Exit");
    }
}
