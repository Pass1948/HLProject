using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine 
{
    // Enemy 상태
    // Idle -> Evaluate -> Decide -> Move / Attack -> End
    // Idle : 턴이 돌아오기 전 대기 상태
    // Evaluate : 턴이 들어오고 플레이어 탐색 / 주변 환경 탐색 및 상황 판단
    // Decide : Evaluate의 결과를 바탕으로 무슨 행동을 할지 결정하는 상태
    // Move / Attack : 전 상태 Decide에서의 결정을 행동 하는 상태 / 공격하거나 이동을 한다.
    // End : 턴을 끝내는 상태 -> 플레이어의 턴으로 감.

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
    public DecideEnemyState DecideEnemyState => decideState;
    public MoveEnemyState MoveState => moveState;
    public AttackEnemyState AttackState => attackState;
    public EndEnemyState EndState => endState;
    
    public EnemyAnimHandler animHandler;
    public EnemyStateMachine(EnemyAnimHandler animHandler)
    {
        this.animHandler = animHandler;

        idleState = new IdleEnemyState(this);
        hitState = new HitEnemyState(this);
        dieState = new DieEnemyState(this);
        evaluateState = new EvaluateEnemyState(this);
        decideState = new DecideEnemyState(this);
        moveState = new MoveEnemyState(this);
        attackState = new AttackEnemyState(this);
        endState = new EndEnemyState(this);

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
