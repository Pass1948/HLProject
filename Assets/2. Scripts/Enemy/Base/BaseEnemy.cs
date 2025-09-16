using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public EnemyModel enemyModel;
    public EnemyController controller;
    public EnemyAnimHandler animHandler;

    protected virtual void OnEnable()
    {
        if (!GameManager.Unit.enemies.Contains(this))
            GameManager.Unit.enemies.Add(this);

        // 데이터 로드 시 null 가드
        var data = GameManager.Data.GetUnit(UnitType.Enemy, Random.Range(2001, 2010));
        if (data == null)
        {
            Debug.LogError("[Enemy] Enemy data not found for given ID range!");
            return; // 데이터 없으면 초기화 스킵
        }

        enemyModel = new EnemyModel();
        enemyModel.InitData(data);

        animHandler = GetComponent<EnemyAnimHandler>();
        controller = GetComponent<EnemyController>();

        // 모델 & 애니메이션 핸들러가 준비된 후에만 초기화
        if (controller != null && animHandler != null)
        {
            controller.Init(enemyModel, animHandler);
        }
        else
        {
            Debug.LogError("[Enemy] EnemyController or AnimHandler is missing on prefab!");
        }
    }

    protected virtual void OnDisable()
    {
        GameManager.Unit.enemies.Remove(this);
    }


}
