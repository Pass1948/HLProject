using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Mouse.SetMouseVar();
    }

    private void Update()
    {
        GameManager.Mouse.MovingMouse();
    }


}
