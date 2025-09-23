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
    public EntityDataGroup entityDataGroup;

    public void Initialize()
    {
        UnityGoogleSheet.LoadAllData();
        entityData = new EntityData();
        entityDataGroup = new EntityDataGroup();
    }
}