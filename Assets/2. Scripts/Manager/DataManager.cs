using DataTable;
using System;
using System.Collections.Generic;
using UGS;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

[Serializable]
public class DataManager : MonoBehaviour
{
    public EntityDataGroup entityDataGroup;
    public RelicDataGroup relicDataGroup;
    public ObstacleDataGroup obstacleDataGroup;
    public StageDataGroup stageDataGroup;
    public BulletDataGroup bulletDataGroup;

    async void Awake()
    {
        await UnityServices.InitializeAsync();
        AnalyticsService.Instance.StartDataCollection();
    }
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