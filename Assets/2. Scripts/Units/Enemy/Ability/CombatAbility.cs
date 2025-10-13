using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAbility : IAbility
{
    public string Name { get; }
    public string Description { get; }
    public void Apply(EnemyController controller)
    {
        throw new System.NotImplementedException();
    }

}
