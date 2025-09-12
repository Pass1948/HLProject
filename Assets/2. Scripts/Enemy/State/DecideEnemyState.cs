using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideEnemyState : BaseEnemyState
{
    public DecideEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        Dictionary<IEnemyState, int> actionScores = new Dictionary<IEnemyState, int>();

        // ���� �ൿ�� ���� ������ �Űܼ� �������� -> �Ϲݸ��ʹ� �̵� / ����
        // ����Ʈ -> �̵� / ���� / + �ٸ���
        // ���� -> �̵� / ���� / + ���� ���ϵ�

        int distance = GetDistanceTarget(stateMachine.Controller.GridPos, stateMachine.Controller.TargetPos);

        //if (distance <= stateMachine.Controller.AttackRange)
        //{
        //    stateMachine.ChangeState(stateMachine.AttackState);
        //}
        //else
        //{
            stateMachine.ChangeState(stateMachine.MoveState);
        //}
    }

    public override void Excute()
    {
        Debug.Log("Decide : Excute");
    }

    public override void Exit()
    {
        Debug.Log("Decide : Exit");
    }

    // Ÿ��(�÷��̾�)���� �Ÿ� ���
    private int GetDistanceTarget(Vector3Int pos, Vector3Int target)
    {
        return Mathf.Abs(pos.x - target.x) + Mathf.Abs(pos.y - target.y);
    }
}
