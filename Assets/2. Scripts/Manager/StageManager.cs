using System.Collections;
using System.Collections.Generic;
using DataTable;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public Stage stage;
    
    public int stageId = 7001;
    private StageData stageData;

    private int currentStageIndex;
    public int GetCurrentStageIndex() => currentStageIndex;
    
    public int id;                                           // 스테이지 앙이디
    public int mapSize;                                      // 맵 사이즈 
    public Dictionary<int, int> obstaclesDict = new();       // 구조물 <아이디, 갯수> 
    public Dictionary<int, int> enemiesDict = new();         // 적 <아이디, 갯수>
    public int eliteCnt;                                     // 부여될 엘리트 수

    public void InitStage(int stageIndex)
    {
        ClearAllData();
        
        stageData = GameManager.Data.stageDataGroup.GetStageData(stageId);
        Debug.Log(stageId);
        mapSize = stageData.size;
        for (int i = 0; i < stageData.enemyList.Count; i++)
        {
            enemiesDict.Add(stageData.enemyList[i], stageData.enemyCount[i]);
        }
        for (int i = 0; i < stageData.obstacleList.Count; i++)
        {
            obstaclesDict.Add(stageData.obstacleList[i], stageData.obstacleCount[i]);
        }
        eliteCnt = stageData.eliteId;
    }

    public void LoadStage(int stageIndex)
    {

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
