using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MacroCommand : BaseCommand
{
    private readonly List<BaseCommand> _children = new List<BaseCommand>();
    public MacroCommand(IEnumerable<BaseCommand> children) { _children.AddRange(children); }

    public override bool CanExecute(CommandContext ctx)
    {
        foreach (var c in _children) if (!c.CanExecute(ctx)) return false;
        return true;
    }

    public override ExecutionRecord Execute(CommandContext ctx)
    {
        if (!CanExecute(ctx)) return ExecutionRecord.Failed(this);
        var recs = new List<ExecutionRecord>(_children.Count);
        foreach (var c in _children)
        {
            var r = c.Execute(ctx);
            if (!r.Success) return ExecutionRecord.Failed(this);
            recs.Add(r);
        }
        return new ExecutionRecord(this, recs, true);
    }

    public override bool SupportsUndo => true;
    public override void Undo(CommandContext ctx, object memento)
    {
        var recs = memento as List<ExecutionRecord>;
        if (recs == null) return;
        for (int i = recs.Count - 1; i >= 0; --i)
            if (recs[i].Command.SupportsUndo) recs[i].Command.Undo(ctx, recs[i].Memento);
    }

    protected override void OnExecute(CommandContext ctx) { }
}
