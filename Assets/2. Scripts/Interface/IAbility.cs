using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    string Name { get; }
    string Description { get; }
    void Apply(EnemyController controller);
}
