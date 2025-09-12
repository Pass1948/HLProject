using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Command�� ��� Ŭ����
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

    // ���� Ȯ�� ����Ʈ
    public virtual bool CanExecute() => true;
}
