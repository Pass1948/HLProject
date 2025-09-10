using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : BaseTurnState
{
    public override void OnEnter()
    {
        // ��� ���·� ����(�Է� ��ٸ�)
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
