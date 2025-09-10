using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    // 현재 턴에 확정된 플랜(배치) — UI/HFSM이 편집
    private readonly List<ICommand> _plan = new();

    // 직전에 실행된 배치(Undo용)
    private readonly Stack<ICommand> _lastBatch = new();

    public Action<ICommand> OnPlanned;         // 플랜에 추가될 때(옵션)
    public Action<ICommand> OnExecuted;        // 실행 후 알림(옵션)
    public Action<ICommand> OnUndo;            // Undo 후 알림(옵션)

    //플랜에 커맨드를 추가(확정). UI/HFSM만 호출
    public void PlanAdd(ICommand cmd)
    {
        if (cmd == null) return;
        _plan.Add(cmd);
        OnPlanned?.Invoke(cmd);
    }

    //플랜의 마지막 항목을 제거(확정 후라도 Resolve 전이면 취소)
    public bool PlanRemoveLast()
    {
        if (_plan.Count == 0) return false;
        _plan.RemoveAt(_plan.Count - 1);
        return true;
    }

    //플랜 비우기(턴 취소/재작성 등)
    public void PlanClear() => _plan.Clear();

    //현재 플랜을 '한 배치'로 실행. 실패 커맨드는 스킵/로깅.
    //실행된 배치는 Undo를 위해 스택에 보관.
    public void ExecutePlan()
    {
        _lastBatch.Clear();
        foreach (var cmd in _plan)
        {
            if (!cmd.CanExecute()) continue;
            cmd.Execute();
            _lastBatch.Push(cmd);
            OnExecuted?.Invoke(cmd);
        }
        _plan.Clear(); // 실행 후 플랜 비움
    }

    //직전 배치를 역순으로 되돌림(디버그/리플레이)
    public void UndoLastBatch()
    {
        while (_lastBatch.Count > 0)
        {
            var cmd = _lastBatch.Pop();
            cmd.Undo();
            OnUndo?.Invoke(cmd);
        }
    }

    //디버그/HUD용: 현재 플랜 길이
    public int PlannedCount => _plan.Count;
}
