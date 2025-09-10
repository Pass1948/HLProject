using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnBasedManager : MonoBehaviour
{
    private void Awake()
    {
        var comp = gameObject.AddComponent<TurnSettingValue>();
    }
}
