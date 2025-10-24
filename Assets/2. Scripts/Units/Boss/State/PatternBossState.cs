using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternBossState : BaseBossState
{
    public PatternBossState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler) : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        Debug.Log("패턴 상태 진입");
    }
    
    public override void Excute() { }
    
    public override void Exit()
    {
        Debug.Log("패턴 상태 종료");
    }
}
