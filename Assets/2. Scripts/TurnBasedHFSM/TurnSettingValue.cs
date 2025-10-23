using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSettingValue : MonoBehaviour
{
    [HideInInspector] public float resetTime = 0f;
    public Ammo fireAmmo;
    [Header("턴별 변동시간")]
    public float turnDelayTime =2f;
    public float actionWindupTime = 1.5f; // 행동 선딜 시간
    public float ActionRecoverTime = 1.5f; // 행동 후딜 시간
    [Header("게임 속도")]
    public int gameTime=1;
    [Header("게임 화면 해상도")]
    public int windowPanelIndex = 0;


    [HideInInspector] public bool IsBasicDeck = false;
    [HideInInspector] public bool IsDiamondDeck = false;
    [HideInInspector] public bool IsHeartDeck = false;
    [HideInInspector] public bool IsSpadeDeck = false;
    [HideInInspector] public bool IsClubDeck = false;
    // == 플레이어 행동 선택 관련 ==
    [HideInInspector] public bool actionSelected = false;  // 플레이어 행동 선택 여부
}
