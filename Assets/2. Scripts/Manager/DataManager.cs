using System;
using System.Collections.Generic;
using UnityEngine;
using DataTable;
using UGS;

[Serializable]
public class DataManager : MonoBehaviour
{
    private EntityData entityData;
    private ObstacleData obstacleData;
    public RelicData relicData;
    public BulletData bulletData;

    public EntityDataGroup entityDataGroup;
    public RelicDataGroup relicDataGroup;
    public BulletDataGroup bulletDataGroup;

    private void Start()
    {
        UnityGoogleSheet.LoadAllData();
    }

    public void Initialize()
    {
        entityDataGroup = new EntityDataGroup();
        relicDataGroup = new RelicDataGroup();
        bulletDataGroup = new BulletDataGroup();
        
    }
}