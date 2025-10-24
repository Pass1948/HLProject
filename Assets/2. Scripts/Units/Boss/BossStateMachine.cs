using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine
{
    // idle -> Evaluate -> Decide -> move
    //  ㄴ>hit -> die             -> attack
    //                            -> pattern
    //                            -> warning
    
    
    
    private IdleBossState idleState;
    private EvaluateBossState evaluateState;
    private DecideBossState decideState;
    private MoveBossState moveState;
    private WarningBossState warningState;
    private AttackBossState attackState;
    private PatternBossState patternState;
    private EndBossState endState;
    private HitBossState hitState;
    private DieBossState dieState;
    
    public IdleBossState IdleState => idleState;
    public EvaluateBossState EvaluateState => evaluateState;
    public DecideBossState DecideState => decideState;
    public MoveBossState MoveState => moveState;
    public WarningBossState WarningState => warningState;
    public AttackBossState AttackState => attackState;
    public PatternBossState PatternState => patternState;
    public EndBossState EndState => endState;
    public HitBossState HitState => hitState;
    public DieBossState DieState => dieState;
    
    private BossController controller;
    public BossController Controller => controller;
    private EnemyAnimHandler animHandler;
    public EnemyAnimHandler  AnimHandler => animHandler;

    public IEnemyState currentState { get; private set; }
    
    public BossStateMachine(BossController controller, EnemyAnimHandler animHandler)
    {
        this.controller = controller;
        this.animHandler = animHandler;

        idleState = new(this, controller, animHandler);
        evaluateState = new(this, controller, animHandler);
        decideState = new(this, controller, animHandler);
        moveState = new(this, controller, animHandler);
        attackState = new(this, controller, animHandler);
        endState = new(this, controller, animHandler);
        hitState = new(this, controller, animHandler);
        dieState = new(this, controller, animHandler);
        
    }

    public void SetPattern(WarningBossState warningState, PatternBossState patternState)
    {
        this.warningState = warningState;
        this.patternState = patternState;
    }
    
    public void Init()
    {
        currentState = idleState;
        currentState.Enter();
    }

    public void ChangeState(IEnemyState State)
    {
        currentState?.Exit();
        currentState = State;
        currentState.Enter();
    }

    public void Excute()
    {
        currentState?.Excute();
    }
}
