using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct ExecutionRecord
{
    public readonly BaseCommand Command;
    public readonly object Memento;
    public readonly bool Success;
    public static ExecutionRecord Failed(BaseCommand cmd) => new ExecutionRecord(cmd, null, false);
    public ExecutionRecord(BaseCommand cmd, object memento, bool success)
    { Command = cmd; Memento = memento; Success = success; }
}
