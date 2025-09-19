using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerTurnState : BaseTurnState
{
    float timer;
    private bool didClose;   // 한 번만 처리하게 하는 게이트
    public PlayerTurnState() { }
    public override void OnEnter()
    {
        timer = turnSetVlaue.resetTime;
        didClose = false;
        // 턴 시작 시 커맨드 초기화후 입력 대기
        GameManager.UI.OpenUI<PaseTurnUI>();
        GameManager.Mouse.ToggleMovePhase();
        GameManager.Unit.Vehicle.vehicleHandler.MountVehicle();
    }

    public override void Tick(float dt)
    {
        // 이미 처리했다면 더 이상 검사하지 않음
        if (didClose) return;
        timer += dt;
        if (timer > turnSetVlaue.turnDelayTime )
        {
            GameManager.UI.CloseUI<PaseTurnUI>();
            GameManager.UI.OpenUI<MainUI>();
            didClose = true;// 한 번만 처리하게 설정
        }
    }
    public override void OnExit()
    {
        // 혹시 못 닫았으면 안전하게 닫아 주기
        if (!didClose) GameManager.UI.CloseUI<PaseTurnUI>();
        GameManager.UI.CloseUI<MainUI>();
        GameManager.Mouse.ToggleMovePhase();
    }
}
