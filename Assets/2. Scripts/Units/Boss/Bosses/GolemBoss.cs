using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

public class GolemBoss : BaseBoss
{
    protected override void SetBossTypePattern()
    {
        base.SetBossTypePattern();
        
        controller.stateMachine.SetPattern(
            new GolemWarningState(controller.stateMachine, controller, animHandler),
            new GolemPatternState(controller.stateMachine, controller, animHandler));
    }
}
