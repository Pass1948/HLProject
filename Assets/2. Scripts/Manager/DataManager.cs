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

    public EntityDataGroup entityDataGroup;
    public RelicDataGroup relicDataGroup;

    public void Initialize()
    {
        UnityGoogleSheet.LoadAllData();
        entityData = new EntityData();
        entityDataGroup = new EntityDataGroup();
        relicData = new RelicData();
        relicDataGroup = new RelicDataGroup();
        GameManager.ItemControl.ItemDataSet();  // 아이템데이터 리스트 초기 세팅
    }
}