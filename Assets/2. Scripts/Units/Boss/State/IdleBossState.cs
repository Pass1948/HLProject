using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBossState : BaseBossState
{
    public IdleBossState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        
    }
    
    public override void Excute()
    {
        if (controller.startTurn == false) return;
        stateMachine.ChangeState(stateMachine.EvaluateState); 
    }
    
    public override void Exit()
    {
        controller.UpdatePlayerPos();
    }
}
