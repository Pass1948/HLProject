using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class TurnBasedManager : MonoBehaviour
{
    [HideInInspector] public TurnStateMachine turnHFSM { get; private set; }
    [HideInInspector] public TurnSettingValue turnSettingValue { get; private set; }

    private readonly Dictionary<Type, ITurnState> _stateCache = new Dictionary<Type, ITurnState>();
    private void Awake()
    {
        turnHFSM = new TurnStateMachine();
        turnHFSM.Set(new IdleState());// �ʱ���� ����
        gameObject.AddComponent<TurnSettingValue>();
        turnSettingValue = GetComponent<TurnSettingValue>();
    }
    private void Update()
    {
        turnHFSM.Tick(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        turnHFSM.FixedTick(Time.fixedDeltaTime);
    }

    // Ÿ Ŭ�������� ���� ���� ��û��
    public void ChangeTo<T>(string reason = null) where T : ITurnState, new()
    {
        var next = GetState<T>();
        turnHFSM.Change(next, reason);
    }

    // ���� �ν��Ͻ� ��������(������ ���� �� ĳ�ÿ� ���)
    public T GetState<T>() where T : ITurnState, new()
    {
        var key = typeof(T);
        if (_stateCache.TryGetValue(key, out var s)) return (T)s;

        // �Ķ���� ���� �����ڷ� ����
        var inst = new T();
        _stateCache[key] = inst;
        return inst;
    }

    // �ʿ� �� �ܺο��� �ʱ�ȭ ����
    public void ResetAll()
    {
        _stateCache.Clear();
    }

}
