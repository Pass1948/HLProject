using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private EnemyStateMachine stateMachine;
    private EnemyAnimHandler animHandler;
    private void Awake()
    {
        animHandler = GetComponent<EnemyAnimHandler>();

        // ���¸ӽ� �Ҵ�, Init �ʱ� ���� Idle��
        stateMachine = new EnemyStateMachine(animHandler);
        stateMachine.Init();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Excute();

        if (Input.GetKeyDown(KeyCode.A))
        {
            stateMachine.ChangeState(stateMachine.EvaluateState);
        }
    }
}
