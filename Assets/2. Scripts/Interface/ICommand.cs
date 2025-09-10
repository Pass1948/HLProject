using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CommandManager의 상태패턴 구조를 참조하여 구조 형성
public interface ICommand
{
    void OnEnter();

    // 취소용
    void Undo();
}
