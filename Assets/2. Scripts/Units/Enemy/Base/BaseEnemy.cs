using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public EnemyModel enemyModel;
    public EnemyController controller;
    public EnemyAnimHandler animHandler;
    public EnemyFloatingUI enemyUI;
    
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
        enemyUI = GetComponent<EnemyFloatingUI>();
        if (controller != null && animHandler != null)
        {
            controller.model = enemyModel;
            controller.animHandler = animHandler;
            controller.InitController(this);
            enemyUI.Init(enemyModel);
        }
        else
        {
            Debug.LogError("애너미 컨트롤러, 애님핸들러 없슴");
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
            // data.eliteName
            //     data.descript
            //         data.
            
        }
    }
}
