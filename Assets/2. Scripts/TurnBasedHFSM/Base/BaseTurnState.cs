using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTurnState : ITurnState
{
    protected TurnBasedManager turnManager => GameManager.TurnBased;
    protected  TurnStateMachine turnHFSM => GameManager.TurnBased.turnHFSM;
    protected bool locked;
    public bool IsLocked => locked;
    public virtual string Name => GetType().Name;

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void Tick(float dt) { }
    public virtual void FixedTick(float fdt) { }

    //상위 상태 전이는 모두 매니저를 통해 수행
    protected void ChangeState<T>(string reason = null) where T : ITurnState, new()
    {
        if (turnManager == null)
        {
            //Debug.LogError("[BaseTurnState] TurnBasedManager가 초기화되지 않았습니다.");
            return;
        }
        turnManager.ChangeTo<T>(reason);
    }
    // 잠금 편의
    protected void Lock() => locked = true;
    protected void Unlock() => locked = false;
}
