using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyState : IEnemyState
{
    protected EnemyStateMachine stateMachine;
    protected EnemyController controller;
    protected EnemyAnimHandler animHandler;

    public BaseEnemyState(EnemyStateMachine stateMachine, EnemyController controller, EnemyAnimHandler animHandler)
    {
        this.stateMachine = stateMachine;
        this.controller = controller;
        this.animHandler = animHandler;
    }

    public abstract void Enter();
    public abstract void Excute();
    public abstract void Exit();

}
