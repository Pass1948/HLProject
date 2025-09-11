using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class TurnBasedManager : MonoBehaviour
{
    // 리퀘스트 테스트
    [HideInInspector] public TurnStateMachine turnHFSM { get; private set; }
    private readonly Dictionary<Type, ITurnState> _stateCache = new Dictionary<Type, ITurnState>();
    private void Awake()
    {
        turnHFSM = new TurnStateMachine();
        turnHFSM.Set(new IdleState());// 초기상태 세팅
        var comp = gameObject.AddComponent<TurnSettingValue>();
    }

    private void Update()
    {
        turnHFSM.Tick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        turnHFSM.FixedTick(Time.fixedDeltaTime);
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

    // 상태 전이
    public void ChangeTo<T>(string reason = null) where T : ITurnState, new()
    {
        var next = GetState<T>();
        turnHFSM.Change(next, reason);
    }

    // 필요 시 외부에서 초기화 리셋
    public void ResetAll()
    {
        _stateCache.Clear();
    }

}
