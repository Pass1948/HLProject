using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnState : BaseTurnState
{
    float timer;
    private bool didClose;
    public EnemyTurnState() { }
    public override void OnEnter()
    {
        GameManager.Event.Publish(EventType.CameraSenter);
        timer = turnSetVlaue.resetTime;
        didClose= false;
        GameManager.UI.OpenUI<PaseTurnUI>();
    }
    public override void Tick(float dt)
    {
        if (didClose) return;
        timer += dt;
        if (timer > turnSetVlaue.turnDelayTime)
        {
            if (turnManager.isCamera == true)
            {
                turnManager.SwitchIsCamera();
            }
            GameManager.UI.CloseUI<PaseTurnUI>();
            turnManager.BeginEnemyPhase();      // 적 턴 시작
            didClose = true;
        }

    }
    public override void OnExit()
    {
        // 혹시 못 닫았으면 안전하게 닫아 주기
        if (!didClose) GameManager.UI.CloseUI<PaseTurnUI>();
    }
}
