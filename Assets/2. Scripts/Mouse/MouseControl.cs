using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseControl : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Mouse.SetMouseVar();
    }
    private void OnMousePoint(InputValue value)
    {
        Vector2 screen = value.Get<Vector2>();
        GameManager.Mouse.UpdatePointerFromScreen(screen);
    }
    private void OnMovementClick(InputValue value)
    {
       GameManager.Mouse.ClickCurrentHover();
    }
    
    private void OnCancel(InputValue value)
    {
        GameManager.Mouse.InputCance();

    }


}
