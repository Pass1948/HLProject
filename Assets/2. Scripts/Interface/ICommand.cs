using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    string Describe();
    bool CanExecute();
    void Execute();  // ���� Ȯ��(��ġ, HP ��)
    void Undo();     // ��Ҹ� ���� �޼���(���û�����)
}
