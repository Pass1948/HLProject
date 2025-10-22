using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

public class GolemBoss : BaseBoss
{
    protected override void SetBossTypePattern()
    {
        base.SetBossTypePattern();

        cooldown = 2;
        patternPower = model.attack * 5;
        patternRange = 3;

        controller.SetController(cooldown, patternPower, patternRange);
        
        controller.stateMachine.SetPattern(
            new GolemWarningState(controller.stateMachine, controller, animHandler),
            new GolemPatternState(controller.stateMachine, controller, animHandler));
        
    }
}
