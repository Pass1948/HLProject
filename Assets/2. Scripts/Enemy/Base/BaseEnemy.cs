using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public EnemyModel enemyModel;
    public EnemyController controller;
    public EnemyAnimHandler animHandler;

    protected virtual void Awake()
    {
        GameManager.Unit.enemies.Add(this);
        enemyModel = new EnemyModel();
        animHandler = GetComponent<EnemyAnimHandler>();
        controller = GetComponent<EnemyController>();
        controller.Init(enemyModel, animHandler);
    }

}
