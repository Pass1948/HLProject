using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementBase : MonoBehaviour 
{
    private Vector3Int _direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    private void MoveToLeftDirection(InputAction.CallbackContext context)
    {
        _direction = Vector3Int.left;
    }
}
