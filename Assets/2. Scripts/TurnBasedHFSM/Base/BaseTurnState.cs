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

    public virtual void OnEnter() { }   // ���� ���� �� �ʱ�ȭ ��
    public virtual void OnExit() { }    // ���� ���� �� ���� ��
    public virtual void Tick(float dt) { }  // �� ������ ���� ��
    public virtual void FixedTick(float fdt) { }

    //���� ���� ���̴� ��� �Ŵ����� ���� ����
    protected void ChangeState<T>(string reason = null) where T : ITurnState, new()
    {
        if (turnManager == null)
        {
            Debug.LogError("[BaseTurnState] TurnBasedManager�� �ʱ�ȭ���� �ʾҽ��ϴ�.");
            return;
        }
        turnManager.ChangeTo<T>(reason);
    }
    // ��� ����
    protected void Lock() => locked = true;
    protected void Unlock() => locked = false;
}
