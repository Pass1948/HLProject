using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// �ϻ��¸ӽſ� ���� ��ƿ�� ��� �뵵
public class TurnBasedManager : MonoBehaviour
{
    [HideInInspector] public TurnStateMachine turnHFSM { get; private set; }

    private void Awake()
    {
        var comp = gameObject.AddComponent<TurnSettingValue>();
    }



}
