using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    // 이벤트별 델리게이트 저장 (타입 안전)
    private readonly Dictionary<EventType, Delegate> _handlers = new Dictionary<EventType, Delegate>();

    // 구독 (페이로드 없는 이벤트)
    public void Subscribe(EventType eventType, Action handler)
    {
        if (_handlers.TryGetValue(eventType, out var del))
        {
            _handlers[eventType] = (Action)del + handler;
        }
        else
        {
            _handlers[eventType] = handler;
        }
    }

    // 구독 해제 (페이로드 없는 이벤트)
    public void Unsubscribe(EventType eventType, Action handler)
    {
        if (!_handlers.TryGetValue(eventType, out var del)) return;
        del = (Action)del - handler;
        if (del == null) _handlers.Remove(eventType);
        else _handlers[eventType] = del;
    }

    // 발행 (페이로드 없는 이벤트)
    public void Publish(EventType eventType)
    {
        if (_handlers.TryGetValue(eventType, out var del))
        {
            (del as Action)?.Invoke();
        }
    }

    // 구독 (페이로드 있는 이벤트) — 타입 안전
    public void Subscribe<T>(EventType eventType, Action<T> handler)
    {
        if (_handlers.TryGetValue(eventType, out var del))
        {
            if (del != null && del.GetType() != typeof(Action<T>))
                throw new InvalidOperationException($"Event {eventType} already registered with different payload type.");
            _handlers[eventType] = (Action<T>)del + handler;
        }
        else
        {
            _handlers[eventType] = handler;
        }
    }

    // 구독 해제 (페이로드 있는 이벤트)
    public void Unsubscribe<T>(EventType eventType, Action<T> handler)
    {
        if (!_handlers.TryGetValue(eventType, out var del)) return;
        if (del != null && del.GetType() != typeof(Action<T>)) return;

        del = (Action<T>)del - handler;
        if (del == null) _handlers.Remove(eventType);
        else _handlers[eventType] = del;
    }

    // 발행 (페이로드 있는 이벤트)
    public void Publish<T>(EventType eventType, T payload)
    {
        if (_handlers.TryGetValue(eventType, out var del))
        {
            (del as Action<T>)?.Invoke(payload);
        }
    }
}
