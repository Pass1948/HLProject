using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� ���� ����ϰ� �ִ� ����
public class IdleEnemyState : BaseEnemyState
{
    public IdleEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler)
    {
    }

    public bool StartTurn { get; set; } = false;

    public override void Enter()
    {
        Debug.Log("Idle : Enter");

    }

    public override void Excute()
    {
        Debug.Log("Idle : Excute");

        if (!StartTurn) return;

        stateMachine.ChangeState(stateMachine.EvaluateState); 

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
