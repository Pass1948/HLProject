using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    /* 이벤트 타입별 멀티캐스트 델리게이트 체인 */
    private readonly Dictionary<EventType, Action<Component, object>> _handlers =
        new Dictionary<EventType, Action<Component, object>>();

    /* 리스너별 구독 목록(역인덱스) → 제거 편의성/정리 */
    private readonly Dictionary<IEventListener, List<(EventType evt, Action<Component, object> cb)>> _reverse =
        new Dictionary<IEventListener, List<(EventType, Action<Component, object>)>>();

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene _, LoadSceneMode __)
    {
        /* 씬 전환 시 파괴된 리스너 정리 */
        RefreshListeners();
    }

    /* IEventListener를 델리게이트로 래핑해 구독(약참조로 자동 해제 지원) */
    public void AddListener(EventType evt, IEventListener listener)
    {
        if (listener == null) return;

        var weak = new WeakReference<IEventListener>(listener);
        Action<Component, object> wrapper = null;

        wrapper = (sender, param) =>
        {
            /* 파괴/소멸 감지: 살아있지 않으면 자기 자신을 구독 해제 */
            if (!weak.TryGetTarget(out var target) || target == null || target.Equals(null))
            {
                UnsubscribeInternal(evt, wrapper);
                if (_reverse.TryGetValue(listener, out var list))
                {
                    list.RemoveAll(t => t.evt == evt && t.cb == wrapper);
                    if (list.Count == 0) _reverse.Remove(listener);
                }
                return;
            }
            target.OnEvent(evt, sender, param);
        };

        if (_handlers.TryGetValue(evt, out var chain)) _handlers[evt] = chain + wrapper;
        else _handlers[evt] = wrapper;

        if (!_reverse.TryGetValue(listener, out var subs))
        {
            subs = new List<(EventType, Action<Component, object>)>();
            _reverse[listener] = subs;
        }
        subs.Add((evt, wrapper));
    }

    /* 델리게이트 직접 구독 경로(원하면 인터페이스 없이 바로 연결) */
    public void AddHandler(EventType evt, Action<Component, object> handler)
    {
        if (handler == null) return;
        if (_handlers.TryGetValue(evt, out var chain)) _handlers[evt] = chain + handler;
        else _handlers[evt] = handler;
        /* 역인덱스는 인터페이스 리스너에만 유지 */
    }

    /* 특정 이벤트에서 해당 리스너 제거 */
    public void RemoveListener(EventType evt, IEventListener listener)
    {
        if (listener == null) return;
        if (!_reverse.TryGetValue(listener, out var list)) return;

        for (int i = list.Count - 1; i >= 0; --i)
        {
            var (e, cb) = list[i];
            if (e != evt) continue;
            UnsubscribeInternal(e, cb);
            list.RemoveAt(i);
        }
        if (list.Count == 0) _reverse.Remove(listener);
    }

    /* 리스너가 구독한 모든 이벤트에서 제거 */
    public void RemoveTarget(IEventListener listener)
    {
        if (listener == null) return;
        if (!_reverse.TryGetValue(listener, out var list)) return;

        foreach (var (evt, cb) in list) UnsubscribeInternal(evt, cb);
        _reverse.Remove(listener);
    }

    /* 이벤트 타입 전체 제거 */
    public void RemoveEvent(EventType evt)
    {
        _handlers.Remove(evt);
        foreach (var kv in _reverse) kv.Value.RemoveAll(t => t.evt == evt);
    }

    /* 이벤트 브로드캐스트: sender는 보통 this, param은 DTO(강타입) 권장 */
    public void Post(EventType evt, Component sender, object param = null)
    {
        if (_handlers.TryGetValue(evt, out var chain)) chain?.Invoke(sender, param);
    }

    /* 파괴/소멸된 유니티 오브젝트 리스너 정리 */
    public void RefreshListeners()
    {
        var dead = new List<IEventListener>();
        foreach (var kv in _reverse)
        {
            var l = kv.Key;
            if (l == null || l.Equals(null)) dead.Add(l);
        }
        foreach (var d in dead) RemoveTarget(d);
    }

    /* 내부 구독 해제 도우미 */
    private void UnsubscribeInternal(EventType evt, Action<Component, object> cb)
    {
        if (_handlers.TryGetValue(evt, out var chain))
        {
            chain -= cb;
            if (chain == null) _handlers.Remove(evt);
            else _handlers[evt] = chain;
        }
    }
}
