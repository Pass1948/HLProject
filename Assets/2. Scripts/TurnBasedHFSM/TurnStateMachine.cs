using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class TurnStateMachine
{
    public ITurnState Current { get; private set; }

    // �ʱ� ���� ������
    public void Set(ITurnState initial)
    {
        if (Current != null) Current.OnExit();
        Current = initial;
        Current?.OnEnter();
    }

    // ���� ��ȯ
    public void Change(ITurnState next, string reason = null)   // reason: "Force"�� ���� ����
    {
        if (next == null || next == Current) return;
        if (Current != null && Current.IsLocked && reason != "Force") return;
        Current?.OnExit();
        Current = next;
        Current?.OnEnter();
    }

    // ���±�� ����
    public void Clear()
    {
        Current?.OnExit();
        Current = null;
    }

    public void Tick(float dt) => Current?.Tick(dt);
    public void FixedTick(float fdt) => Current?.FixedTick(fdt);
}
