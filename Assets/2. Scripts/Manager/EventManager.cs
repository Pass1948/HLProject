using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    /* �̺�Ʈ Ÿ�Ժ� ��Ƽĳ��Ʈ ��������Ʈ ü�� */
    private readonly Dictionary<EventType, Action<Component, object>> _handlers =
        new Dictionary<EventType, Action<Component, object>>();

    /* �����ʺ� ���� ���(���ε���) �� ���� ���Ǽ�/���� */
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
        /* �� ��ȯ �� �ı��� ������ ���� */
        RefreshListeners();
    }

    /* IEventListener�� ��������Ʈ�� ������ ����(�������� �ڵ� ���� ����) */
    public void AddListener(EventType evt, IEventListener listener)
    {
        if (listener == null) return;

        var weak = new WeakReference<IEventListener>(listener);
        Action<Component, object> wrapper = null;

        wrapper = (sender, param) =>
        {
            /* �ı�/�Ҹ� ����: ������� ������ �ڱ� �ڽ��� ���� ���� */
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

    /* ��������Ʈ ���� ���� ���(���ϸ� �������̽� ���� �ٷ� ����) */
    public void AddHandler(EventType evt, Action<Component, object> handler)
    {
        if (handler == null) return;
        if (_handlers.TryGetValue(evt, out var chain)) _handlers[evt] = chain + handler;
        else _handlers[evt] = handler;
        /* ���ε����� �������̽� �����ʿ��� ���� */
    }

    /* Ư�� �̺�Ʈ���� �ش� ������ ���� */
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

    /* �����ʰ� ������ ��� �̺�Ʈ���� ���� */
    public void RemoveTarget(IEventListener listener)
    {
        if (listener == null) return;
        if (!_reverse.TryGetValue(listener, out var list)) return;

        foreach (var (evt, cb) in list) UnsubscribeInternal(evt, cb);
        _reverse.Remove(listener);
    }

    /* �̺�Ʈ Ÿ�� ��ü ���� */
    public void RemoveEvent(EventType evt)
    {
        _handlers.Remove(evt);
        foreach (var kv in _reverse) kv.Value.RemoveAll(t => t.evt == evt);
    }

    /* �̺�Ʈ ��ε�ĳ��Ʈ: sender�� ���� this, param�� DTO(��Ÿ��) ���� */
    public void Post(EventType evt, Component sender, object param = null)
    {
        if (_handlers.TryGetValue(evt, out var chain)) chain?.Invoke(sender, param);
    }

    /* �ı�/�Ҹ�� ����Ƽ ������Ʈ ������ ���� */
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

    /* ���� ���� ���� ����� */
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
