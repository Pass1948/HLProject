using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TurnStateMachine
{
    public ITurnState Current { get; private set; }

    public void Change(ITurnState next, string reason = null)   // reason: "Force"면 강제 전이
    {
        if (next == null || next == Current) return;
        if (Current != null && Current.IsLocked && reason != "Force") return;
        Current?.OnExit();
        Current = next;
        Current?.OnEnter();
    }

    public void Tick(float dt) => Current?.Tick(dt);
    public void FixedTick(float fdt) => Current?.FixedTick(fdt);
}
