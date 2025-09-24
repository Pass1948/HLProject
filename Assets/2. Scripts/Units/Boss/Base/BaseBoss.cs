using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBoss : BaseEnemy
{
    public int phase { get; protected set; } = 1;
    
    protected virtual void InitBossPattern(){}
    public virtual void ExecutePattern(){}
    protected void ChangePhase(){}
    protected virtual void OnPhaseChanged(){}
}
