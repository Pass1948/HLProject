using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTest : MonoBehaviour
{
    [SerializeField] AttackTestController controller;

    void Awake()
    {
        if (!controller) controller = FindAnyObjectByType<AttackTestController>();
    }

    void OnMouseDown()
    {
        controller.SelectTarget(this);
    }
}
