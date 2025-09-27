using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public EnemyModel enemyModel;
    public EnemyController controller;
    public EnemyAnimHandler animHandler;
    protected virtual void Start()
    {
        
    }

    public void InitEnemy(EntityData data, EnemyType enemyType)
    {
        if (!GameManager.Unit.enemies.Contains(this))
            GameManager.Unit.enemies.Add(this);
        
        enemyModel = new EnemyModel();
        enemyModel.InitData(data, enemyType);
        animHandler = GetComponent<EnemyAnimHandler>();
        controller = GetComponent<EnemyController>();
        if (controller != null && animHandler != null)
        {
            controller.model = enemyModel;
            controller.animHandler = animHandler;
            controller.InitController(this);
        }
        else
        {
            Debug.LogError("[Enemy] EnemyController or AnimHandler is missing on prefab!");
        }
    }

    public void ChenageAttribute()
    {
        if(enemyModel.attri == EnemyAttribute.High)
        {
            enemyModel.attri = EnemyAttribute.Low;
        }
        else
        {
            enemyModel.attri = EnemyAttribute.High;
        }
    }

    public void SetElite(int stageId)
    {
        if (stageId >= 7001 && stageId <= 7008)
        {
            EliteData data = GameManager.Data.eliteDataGroup.GetEliteData(Random.Range(5001, 5004));
            // data.eliteName
            //     data.descript
            //         data.
            
        }
    }
}
