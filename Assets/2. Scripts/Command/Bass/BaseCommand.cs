using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Command의 상속 클래스
public abstract class BaseCommand
{
    public virtual bool CanExecute(CommandContext ctx) => true;

    // 템플릿: 캡처 → 실행 → 기록
    public virtual ExecutionRecord Execute(CommandContext ctx)
    {
        if (!CanExecute(ctx)) return ExecutionRecord.Failed(this);
        var m = CaptureMemento(ctx);
        OnExecute(ctx);
        return new ExecutionRecord(this, m, true);
    }

    protected abstract void OnExecute(CommandContext ctx);
    protected virtual object CaptureMemento(CommandContext ctx) => null;

    public virtual bool SupportsUndo => false;
    public virtual void Undo(CommandContext ctx, object memento) { /* 필요 시 오버라이드 */ }
}
