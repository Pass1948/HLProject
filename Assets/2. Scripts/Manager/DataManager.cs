using System;
using System.Collections.Generic;
using UnityEngine;
using DataTable;
using UGS;

[Serializable]
public class DataManager : MonoBehaviour
{
    public EntityDataGroup entityDataGroup;
    public RelicDataGroup relicDataGroup;
    public ObstacleDataGroup obstacleDataGroup;
    public StageDataGroup stageDataGroup;
    public BulletDataGroup bulletDataGroup;

    private void Start()
    {
        UnityGoogleSheet.LoadAllData();
    }

    public void Initialize()
    {
        entityDataGroup = new EntityDataGroup();
        relicDataGroup = new RelicDataGroup();
        obstacleDataGroup = new ObstacleDataGroup();
        stageDataGroup = new StageDataGroup();
        bulletDataGroup = new BulletDataGroup();
        
    }
}