using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine 
{
    public IEnemyState currentState { get; private set; }

    private IdleEnemyState idleState;
    private HitEnemyState hitState;
    private DieEnemyState dieState;
    private EvaluateEnemyState evaluateState;
    private DecideEnemyState decideState;
    private MoveEnemyState moveState;
    private AttackEnemyState attackState;
    private EndEnemyState endState;

    public IdleEnemyState IdleState => idleState;
    public HitEnemyState HitState => hitState;
    public DieEnemyState DieState => dieState;
    public EvaluateEnemyState EvaluateState => evaluateState;
    public DecideEnemyState DecideState => decideState;
    public MoveEnemyState MoveState => moveState;
    public AttackEnemyState AttackState => attackState;
    public EndEnemyState EndState => endState;
    

    private EnemyAnimHandler animHandler;
    public EnemyAnimHandler AnimHandler => animHandler;

    private EnemyController controller;
    public EnemyController Controller => controller;

    public EnemyStateMachine(EnemyAnimHandler animHandler, EnemyController controller)
    {
        this.animHandler = animHandler;
        this.controller = controller;

        idleState = new IdleEnemyState(this, Controller, AnimHandler);
        hitState = new HitEnemyState(this, Controller, AnimHandler);
        dieState = new DieEnemyState(this, Controller, AnimHandler);
        evaluateState = new EvaluateEnemyState(this, Controller, AnimHandler);
        decideState = new DecideEnemyState(this, Controller, AnimHandler);
        moveState = new MoveEnemyState(this, Controller, AnimHandler);
        attackState = new AttackEnemyState(this, Controller, AnimHandler);
        endState = new EndEnemyState(this, Controller, AnimHandler);
    }

    public void Init()
    {
        currentState = idleState;
        currentState?.Enter();
    }

    public void ChangeState(IEnemyState state)
    {
        currentState?.Exit();
        currentState = state;
        currentState?.Enter();
    }

    public void Excute()
    {
        currentState?.Excute();
    }
}
