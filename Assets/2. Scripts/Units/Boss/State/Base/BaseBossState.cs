using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBossState : IEnemyState
{
    protected BossStateMachine stateMachine;
    protected BossController controller;
    protected EnemyAnimHandler animHandler;

    public BaseBossState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler)
    {
        this.stateMachine = stateMachine;
        this.controller = controller;
        this.animHandler = animHandler;
    }

    public abstract void Enter();
    public abstract void Excute();
    public abstract void Exit();
}
