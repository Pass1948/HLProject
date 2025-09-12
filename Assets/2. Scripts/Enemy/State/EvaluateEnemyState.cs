using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluateEnemyState : BaseEnemyState
{
    // Enemy�� ���� ���� ���۵Ǿ��� �� ��ȯ�� ����
     
    public EvaluateEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        Debug.Log("Evaluate : Enter");

        // �ൿ ���� ��
        // �ൿ�� �´� ���·� ��ȯ
        // ����
        // if (���� ���� ������ ������ ���)
           //{
           //     // ���� ���� ��ȯ
           //    stateMachine.ChangeState(stateMachine.AttackState);
           //}
           //else  // �ƴҰ��
           //{
           //     // �̵����� ��ȯ
           //    stateMachine.ChangeState(stateMachine.MoveState);
           //} 
        
               stateMachine.ChangeState(stateMachine.DecideState);

        //int distance = GetDistanceTarget()

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
    private int GetDistanceTarget(Vector2Int pos, Vector2Int target)
    {
        return Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y);
    }


}
