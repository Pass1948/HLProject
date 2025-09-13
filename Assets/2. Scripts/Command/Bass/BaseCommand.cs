using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Command의 상속 클래스
public abstract class BaseCommand : ICommand
{
    public string Name { get; }
    protected readonly CommandContext Context;

    protected BaseCommand(string name)
    {
        Name = name;
    }

    public abstract void Execute();

    // 선택 확장 포인트
    public virtual bool CanExecute() => true;
}
