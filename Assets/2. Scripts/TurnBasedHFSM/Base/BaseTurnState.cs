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
    protected void ChangeState(ITurnState next)
    {
        var fsm = turnHFSM;
        if (fsm == null) return;
        fsm.Change(next);
    }
}
