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

        bossName = model.unitName;
        bossDescription = $"2턴 마다 강력한\n내려찍기를 준비합니다.\n\n발차기를 이용해 균형을 잃게하여\n기절 시킬 수 있습니다.";

        controller.SetController(cooldown, patternPower, patternRange);
        
        controller.stateMachine.SetPattern(
            new GolemWarningState(controller.stateMachine, controller, animHandler),
            new GolemPatternState(controller.stateMachine, controller, animHandler));
        
    }
    
    public override void ChenageAttribute()
    {
        base.ChenageAttribute();
        
        controller.StunStart();
    }
}
