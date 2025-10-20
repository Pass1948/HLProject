using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningBossState : BaseBossState
{
    public MapManager map;
    
    public WarningBossState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler) 
        : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        map = GameManager.Map;
    }

    public override void Exit()
    {
    }

    public override void Excute()
    {
    }

    public void WarningArea()
    {
        
    }
}
