using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectSystem : MonoBehaviour
{

    // 상태이상 적용 순서: "1.기절 2.출혈 3.화상 4.재생 5.감속 6.가속 7.실명 8.광폭 9.빙결 10.혼란"
    public static class StatusApply
    {
        public static readonly StatusId[] ApplyOrder =
        {
            StatusId.Stun,
            StatusId.Bleed,
            StatusId.Burn,
            StatusId.Regen,
            StatusId.Slow,
            StatusId.Haste,
            StatusId.Blind,
            StatusId.Berserk,
            StatusId.Freeze,
            StatusId.Confuse
        };
    }
    
    
    
    
    
    
}
