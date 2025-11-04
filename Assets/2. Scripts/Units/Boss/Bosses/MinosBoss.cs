using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinosBoss : BaseBoss
{
    protected override void SetBossTypePattern()
    {
        base.SetBossTypePattern();

        cooldown = 4;
        patternPower = model.attack * 5;
        patternRange = 3;

        bossName = model.unitName;
        bossDescription = $"돌진시 단단한 돌기둥에 부딪혀 잠깐 기절한다.";

        controller.SetController(true, cooldown, patternPower, patternRange);
        
        controller.stateMachine.SetPattern(
            new MinosWarningState(controller.stateMachine, controller, animHandler),
            new MinosPatternState(controller.stateMachine, controller, animHandler));
        
    }
    
    public override void ChenageAttribute()
    {
        base.ChenageAttribute();
        
        // controller.StunStart(); // 돌기둥에 부딪혔을때 기절하는 걸로 옮기기
    }
}
