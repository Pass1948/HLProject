using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine
{
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

        idleState = new IdleBossState(this, controller, animHandler);
        evaluateState = new EvaluateBossState(this, controller, animHandler);
        decideState = new DecideBossState(this, controller, animHandler);
        moveState = new MoveBossState(this, controller, animHandler);
        warningState = new WarningBossState(this, controller, animHandler);
        attackState = new AttackBossState(this, controller, animHandler);
        patternState = new PatternBossState(this, controller, animHandler);
        endState = new EndBossState(this, controller, animHandler);
        hitState = new HitBossState(this, controller, animHandler);
        dieState = new DieBossState(this, controller, animHandler);
        
    }
    
    public void Init()
    {
        
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
