using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecideEnemyState : BaseEnemyState
{
    public DecideEnemyState(EnemyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Dictionary<IEnemyState, int> actionScores = new Dictionary<IEnemyState, int>();

        // 무슨 행동을 할지 점수를 매겨서 결정하자 -> 일반몬스터는 이동 / 공격
        // 엘리트 -> 이동 / 공격 / + 다른거
        // 보스 -> 이동 / 공격 / + 여러 패턴들
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
