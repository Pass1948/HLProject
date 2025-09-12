using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Command의 상속 클래스
public abstract class BaseCommand
{
    public string Name { get; }
    protected readonly CommandContext Context;

    protected BaseCommand(string name, CommandContext context)
    {
        Name = name;
        Context = context;
    }

    public abstract void Execute();

    // 선택 확장 포인트
    public virtual bool CanExecute() => true;
}
