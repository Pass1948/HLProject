using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public BossModel model;
    public EnemyAnimHandler animHandler;
    public BossStateMachine StateMachine;

    private BaseBoss baseBoss;
    
    public void InitController(BaseBoss boss)
    {
        baseBoss = boss;

        StateMachine = new BossStateMachine(this, animHandler);
    }
}
