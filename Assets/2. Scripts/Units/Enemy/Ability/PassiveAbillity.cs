using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveAbility : IAbility
{
    // 강화, 철벽, 광폭, 저격
    
    public string Name { get; }
    public string Description { get; }
    public void Apply(EnemyController controller)
    {
    }

}
