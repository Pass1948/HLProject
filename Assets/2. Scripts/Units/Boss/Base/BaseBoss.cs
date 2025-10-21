using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

public class BaseBoss : MonoBehaviour
{
    public BossModel model;
    public BossController controller;
    public EnemyAnimHandler animHandler;

    public int cooldown;
    public int patternPower;
    
    public void InitBoss(EntityData data)
    {
        GameManager.Unit.boss = this;
        
        model = new BossModel();
        model.InitData(data);
        animHandler = GetComponent<EnemyAnimHandler>();
        controller = GetComponent<BossController>();
        if (controller != null && animHandler != null)
        {
            controller.model = model;
            controller.animHandler = animHandler;
            controller.InitController(this);
        }
        else
        {
            Debug.LogError("보스 컨트롤러, 애님핸들러 없슴");
        }
        
        SetBossTypePattern();
    }

    protected virtual void SetBossTypePattern()
    {
    }
    
}
