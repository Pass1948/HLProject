using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyState : IEnemyState
{
    protected EnemyStateMachine stateMachine;

    public BaseEnemyState(EnemyStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Excute();
    public abstract void Exit();

}
