using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffAbility : IAbility
{
    // 신속화, 슬로우, 무리 강화
    
    public string Name { get; }
    public string Description { get; }
    public void Apply(EnemyController controller)
    {
        throw new System.NotImplementedException();
    }

}
