using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluateEnemyState : BaseEnemyState
{
    // Enemy�� ���� ���� ���۵Ǿ��� �� ��ȯ�� ����
     
    public EvaluateEnemyState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Evaluate : Enter");

        // �ൿ ���� ��
        // �ൿ�� �´� ���·� ��ȯ
        // ����
        /* if (���� ���� ������ ������ ���)
           {
                // ���� ���� ��ȯ
               stateMachine.ChangeState(stateMachine.AttackState);
           }
           else  // �ƴҰ��
           {
                // �̵����� ��ȯ
               stateMachine.ChangeState(stateMachine.MoveState);
           } 
        */
    }

    public override void Excute()
    {
        Debug.Log("Evaluate : Excute");

        // Do nothing!!
    }

    public override void Exit()
    {
        Debug.Log("Evaluate : Exit");
    }


    // Ÿ��(�÷��̾�)���� �Ÿ� ���

}
