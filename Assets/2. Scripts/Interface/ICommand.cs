using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    string Describe();
    bool CanExecute();
    void Execute();  // 상태 확정(위치, HP 등)
    void Undo();     // 취소를 위한 메서드(선택사항임)
}
