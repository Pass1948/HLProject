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
        gameObject.AddComponent<TurnSettingValue>();
        turnSettingValue = GetComponent<TurnSettingValue>();
    }

    private void Start()
    {
        // 초기 상태 설정
        SetTo<IdleState>();
    }

    private void Update()
    {
        turnHFSM.Tick(Time.deltaTime);
    }
    private void FixedUpdate()
    {
        turnHFSM.FixedTick(Time.fixedDeltaTime);
    }
    public void SetTo<T>(string reason = null) where T : ITurnState, new()
    {
        var next = GetState<T>();
        turnHFSM.Set(next);
    }

    // 타 클래스에서 상태 전이 요청용
    public void ChangeTo<T>(string reason = null) where T : ITurnState, new()
    {
        var next = GetState<T>();
        turnHFSM.Change(next, reason);
    }

    // 상태 인스턴스 가져오기(없으면 생성 후 캐시에 등록)
    public T GetState<T>() where T : ITurnState, new()
    {
        var key = typeof(T);
        if (_stateCache.TryGetValue(key, out var s)) return (T)s;

        // 파라미터 없는 생성자로 생성
        var inst = new T();
        _stateCache[key] = inst;
        return inst;
    }

    // 필요 시 외부에서 초기화 리셋
    public void ResetAll()
    {
        _stateCache.Clear();
    }

}
