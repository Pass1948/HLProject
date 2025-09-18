using System.Collections;
using System.Collections.Generic;
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

    public void InitEnemy(UnitData data)
    {
        if (!GameManager.Unit.enemies.Contains(this))
            GameManager.Unit.enemies.Add(this);
        // 데이터 로드 시 null 가드

        enemyModel = new EnemyModel();
        enemyModel.InitData(data);
        animHandler = GetComponent<EnemyAnimHandler>();
        controller = GetComponent<EnemyController>();
        // 모델 & 애니메이션 핸들러가 준비된 후에만 초기화
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
    protected virtual void OnDestroy()
    {
        GameManager.Unit.enemies.Remove(this);
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
}
