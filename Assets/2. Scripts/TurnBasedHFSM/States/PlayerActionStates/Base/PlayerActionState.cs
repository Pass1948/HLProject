using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionState : BaseTurnState
{
    protected enum Phase { Windup, Execute, Recover, Done }
    protected Phase currentPhase;

    // 하위 클래스에서 현재 단계에 따른 로직 구현
    public override void OnEnter()
    {
        currentPhase = Phase.Windup;
        OnWindupEnter();
    }

    public override void Tick(float dt)
    {
        switch (currentPhase)
        {
            case Phase.Windup:
                if (OnWindupTick(dt)) ChangePhase(Phase.Execute);
                break;
            case Phase.Execute:
                if (OnExecuteTick(dt)) ChangePhase(Phase.Recover);
                break;
            case Phase.Recover:
                if (OnRecoverTick(dt)) ChangePhase(Phase.Done);
                break;
            case Phase.Done:
                OnActionDone();
                break;
        }
    }

    protected virtual void OnWindupEnter() { }
    protected virtual bool OnWindupTick(float dt) => true;   // true 반환 시 다음 단계로
    protected virtual bool OnExecuteTick(float dt) => true;
    protected virtual bool OnRecoverTick(float dt) => true;
    protected virtual void OnActionDone()
    {
        // 기본적으로 행동이 끝나면 턴 상태기로 복귀
        ChangeState<PlayerChooseState>("Action Finished");
    }

    private void ChangePhase(Phase next)
    {
        currentPhase = next;
        // 각 단계 진입시 추가 로직이 필요하면 여기에 작성
    }
}
