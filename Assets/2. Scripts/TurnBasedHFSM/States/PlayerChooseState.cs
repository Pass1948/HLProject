using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이후 추가로 ui작업하여 2페이즈로 플레이어 인식할수있게 작업하기
public class PlayerChooseState : BaseTurnState
{
   public PlayerChooseState() { }
    float timer;
    private bool didClose;
    public override void OnEnter()
    {
        timer = turnSetVlaue.resetTime;
        didClose = false;
        GameManager.Mouse.isMouse = true;
        GameManager.Mouse.isShowRange = false;
    }

    public override void Tick(float dt)
    {
        if (didClose) return;
        timer += dt;
        if (timer > turnSetVlaue.turnDelayTime)
        {
            GameManager.UI.OpenUI<MainUI>();
            didClose = true;// 한 번만 처리하게 설정
        }
    }
    public override void OnExit()
    {
        // 혹시 못 닫았으면 안전하게 닫아 주기
        if (!didClose) GameManager.UI.CloseUI<PaseTurnUI>();
    }
}
