using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSettingValue : MonoBehaviour
{
    [HideInInspector] public float resetTime = 0f;

    [Header("�Ϻ� �����ð�")]
    public float turnDelayTime = 2f;

    // == �÷��̾� �ൿ ���� ���� ==
    [HideInInspector] public bool actionSelected = false;  // �÷��̾� �ൿ ���� ����
}
