using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : BaseTurnState
{
    public override void OnEnter()
    {
        // 대기 상태로 진입(입력 기다림)
        turnHFSM.Set();
    }

    public override void OnExit()
    {
        turnHFSM.Clear();
    }

    public override void Tick(float dt)
    {
        turnHFSM.Tick(dt);
    }
}
