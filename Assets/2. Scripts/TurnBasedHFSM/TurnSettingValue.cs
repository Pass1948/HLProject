using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSettingValue : MonoBehaviour
{
    [HideInInspector] public float resetTime = 0f;

    [Header("턴별 변동시간")]
    public float turnDelayTime =2f;

    // == 플레이어 행동 선택 관련 ==
    [HideInInspector] public bool actionSelected = false;  // 플레이어 행동 선택 여부
}
