using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieBossState : BaseBossState
{
    public DieBossState(BossStateMachine stateMachine, BossController controller, EnemyAnimHandler animHandler)
        : base(stateMachine, controller, animHandler) { }

    public override void Enter()
    {
        if (controller.isDie)
        {
            animHandler.OnDie();
            Exit();
        }
    }

    public override void Exit()
    {
        GameManager.Map.SetObjectPosition(
            controller.GridPos.x, 
            controller.GridPos.y, 
            TileID.Terrain
        );
        // GameManager.Unit.enemies.Remove(controller.baseBoss);
        controller.baseBoss.gameObject.SetActive(false);
        //controller.baseEnemy.gameObject.Destroy();
    }

    public override void Excute()
    {
        throw new System.NotImplementedException();
    }
}
