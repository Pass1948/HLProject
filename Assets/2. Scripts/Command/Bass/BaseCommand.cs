using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Command�� ��� Ŭ����
public abstract class BaseCommand
{
    public virtual bool CanExecute(CommandContext ctx) => true;

    // ���ø�: ĸó �� ���� �� ���
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
    public virtual void Undo(CommandContext ctx, object memento) { /* �ʿ� �� �������̵� */ }
}
