using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveEnemyState : BaseEnemyState
{
    public MoveEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public Pathfinding pathfind;

    public override void Enter()
    {
        Debug.Log("Move : Enter");

        pathfind = new Pathfinding(MapManager.instance.tilemap);

        pathfind.FindPath(stateMachine.Controller.GridPos, stateMachine.Controller.TargetPos);
    }

    public override void Excute()
    {
        Debug.Log("Move : Excute");

        // 움직임 구현



    }

    public override void Exit()
    {
        Debug.Log("Move : Exit");

    }
}
