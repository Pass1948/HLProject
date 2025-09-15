using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTurnState : ITurnState
{
    protected TurnBasedManager turnManager => GameManager.TurnBased;
    protected  TurnStateMachine turnHFSM => GameManager.TurnBased.turnHFSM;

    protected TurnSettingValue turnSetVlaue=> GameManager.TurnBased.turnSettingValue;

    protected bool locked;
    public bool IsLocked => locked;
    public virtual string Name => GetType().Name;

    public virtual void OnEnter() { }   // 상태 진입 시 초기화 용
    public virtual void OnExit() { }    // 상태 종료 시 정리 용
    public virtual void Tick(float dt) { }  // 매 프레임 갱신 용
    public virtual void FixedTick(float fdt) { }

    //상위 상태 전이는 모두 매니저를 통해 수행
    protected void ChangeState<T>(string reason = null) where T : ITurnState, new()
    {
        if (turnManager == null)
        {
            Debug.LogError("[BaseTurnState] TurnBasedManager가 초기화되지 않았습니다.");
            return;
        }
        turnManager.ChangeTo<T>(reason);
    }
    // 잠금 편의
    protected void Lock() => locked = true;
    protected void Unlock() => locked = false;
}
