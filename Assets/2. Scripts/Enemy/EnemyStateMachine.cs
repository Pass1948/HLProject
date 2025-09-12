using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine 
{
    // Enemy ����
    // Idle -> Evaluate -> Decide -> Move / Attack -> End
    // Idle : ���� ���ƿ��� �� ��� ����
    // Evaluate : ���� ������ �÷��̾� Ž�� / �ֺ� ȯ�� Ž�� �� ��Ȳ �Ǵ�
    // Decide : Evaluate�� ����� �������� ���� �ൿ�� ���� �����ϴ� ����
    // Move / Attack : �� ���� Decide������ ������ �ൿ �ϴ� ���� / �����ϰų� �̵��� �Ѵ�.
    // End : ���� ������ ���� -> �÷��̾��� ������ ��.

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
