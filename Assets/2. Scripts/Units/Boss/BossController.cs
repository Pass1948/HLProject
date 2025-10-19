using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private BaseBoss baseBoss;
    public BossModel model;
    public EnemyAnimHandler animHandler;
    public BossStateMachine stateMachine;

    public Vector3Int GridPos { get; set; }
    public Vector3Int TargetPos { get; set; }

    public int moveRange;
    public int minAttackRange;
    public int maxAttackRange;
    public bool isDie;
    public bool isDone;
    public bool startTurn = false;

    public float moveDuration = 0.2f;

    private void OnEnable()
    {
        GameManager.Event.Subscribe(EventType.EnemyTurnStart, StartTurn);
    }

    private void OnDisable()
    {
        GameManager.Event.Unsubscribe(EventType.EnemyTurnStart, StartTurn);
    }

    public void InitController(BaseBoss boss)
    {
        baseBoss = boss;
        moveRange = model.moveRange;
        minAttackRange = model.minAttackRange;
        maxAttackRange = model.maxAttackRange;
        isDie = model.isDie;

        RunStateMachine();
        
    }

    private void RunStateMachine()
    {
        stateMachine = new BossStateMachine(this, animHandler);
        if (stateMachine == null) Debug.LogError("보스 스테이트 머신 없슴");
        stateMachine.Init();
    }

    public void SetPosition(int x, int y)
    {
        GridPos = new Vector3Int(x, y, 0);
    }
    
    public void UpdatePlayerPos()
    {
        Vector2Int player = GameManager.Map.GetPlayer2Position();

        if (player.x != -1) // 찾았으면
        {
            TargetPos = new Vector3Int(player.x, player.y, 0);
        }
    }
    
    private void StartTurn()
    {
        throw new System.NotImplementedException();
    }

    private void Update()
    {
        stateMachine?.Excute();
    }
}
