using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;
 
public class Stage
{
    private int gateId = 6001;
    private GateData gate;

    private int stageId;
    private StageData stage;

    private int currentStageIndex;
    public int GetCurrentStageIndex() => currentStageIndex;
    
    public int id;                                           // 스테이지 앙이디
    public int mapSize;                                      // 맵 사이즈 
    public Dictionary<int, int> obstaclesDict = new();       // 구조물 <아이디, 갯수> 
    public Dictionary<int, int> enemiesDict = new();         // 적 <아이디, 갯수>
    public int eliteCnt;                                     // 부여될 엘리트 수

    public void InitStage(int stageIndex)
    {
        gate = GameManager.Data.gateDataGroup.GetGateData(gateId);

        LoadStage(stageIndex);
        
    }

    public void LoadStage(int stageIndex)
    {
        ClearAllData();
        
        stageId = gate.stageList[stageIndex];
        stage = GameManager.Data.stageDataGroup.GetStageData(stageId); 
        mapSize = stage.size;
        for (int i = 0; i < stage.enemyList.Count; i++)
        {
            enemiesDict.Add(stage.enemyList[i], stage.enemyCount[i]);
        }
        for (int i = 0; i < stage.obstacleList.Count; i++)
        {
            obstaclesDict.Add(stage.obstacleList[i], stage.obstacleCount[i]);
        }
        eliteCnt = stage.eliteId;
    }
    
    public void NextStage()
    {
        currentStageIndex++;
    }
    
    public void ClearAllData()
    {
        obstaclesDict.Clear();
        enemiesDict.Clear();
    }

}
 