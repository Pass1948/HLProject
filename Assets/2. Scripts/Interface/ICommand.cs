using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CommandManager�� �������� ������ �����Ͽ� ���� ����
public interface ICommand
{
    void OnEnter();

    // ��ҿ�
    void Undo();
}
