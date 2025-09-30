using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMouseState
{
    void Enter();
    void Exit();
    void OnCellClick(Vector3Int cell);
}
