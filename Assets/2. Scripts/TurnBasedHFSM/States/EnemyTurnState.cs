using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnState : BaseTurnState
{
    float timer;
    public EnemyTurnState() { }
    public override void OnEnter()
    {
        timer = turnSetVlaue.resetTime;
        GameManager.UI.OpenUI<PaseTurnUI>();

    }
    public override void Tick(float dt)
    {
        timer += dt;
        if (timer > turnSetVlaue.turnDelayTime)
        {
            GameManager.UI.CloseUI<PaseTurnUI>();
            GameManager.Event.Publish(EventType.EnemyTurnStart);
            /* if ()// 몬스터 행동 체크 조건으로 실행
       {
           // ChangeState<IdleState>(); // 몬스터 행동 끝나면 IdleState로
           // ChangeState<ClearCheckState>(); //  플래이어 죽을시 ClearCheckState로
       }*/
        }

    }
}
