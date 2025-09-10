using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 턴상태머신에 쓰일 유틸들 담는 용도
public class TurnBasedManager : MonoBehaviour
{
    [HideInInspector] public TurnStateMachine turnHFSM { get; private set; }

    private void Awake()
    {
        var comp = gameObject.AddComponent<TurnSettingValue>();
    }



}
