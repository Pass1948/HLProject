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
        // ������ �ε� �� null ����

        enemyModel = new EnemyModel();
        enemyModel.InitData(data, enemyType);
        animHandler = GetComponent<EnemyAnimHandler>();
        controller = GetComponent<EnemyController>();
        // �� & �ִϸ��̼� �ڵ鷯�� �غ�� �Ŀ��� �ʱ�ȭ
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


    public void ChenageAttribute()  // ������(�Ӽ�����)
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
