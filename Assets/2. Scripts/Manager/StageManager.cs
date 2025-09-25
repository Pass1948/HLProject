using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private GateData currentGate;
    // private int stageData = ;
    private Dictionary<int, Stage> stageDB = new();

    public void LoadGate(int gateId)
    {
        currentGate = GameManager.Data.gateDataGroup.GetGateData(gateId);
        if (currentGate == null)
        {
            Debug.Log("게이트 엄슴");
            return;
        }
    }
    
    public void Init(List<Stage> stageList)
    {
        foreach (Stage stage in stageList)
        {
            stageDB[stage.id] = stage;
        }
    }
    
    public void LoadStage(int stageId)
    {
        if (!stageDB.ContainsKey(stageId))
        {
            Debug.Log("스테이지 없슴");
            return;
        }
    }

}
