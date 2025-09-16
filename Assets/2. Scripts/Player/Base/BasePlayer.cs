using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{
    public PlayerModel playerModel;
    public MovementController controller;
    public PlayerAnimHandler animHandler;

    protected virtual void Awake()
    {
        GameManager.Unit.Player = this;
        playerModel = new PlayerModel();
        animHandler = GetComponent<PlayerAnimHandler>();
        controller = GetComponent<MovementController>();
        controller.isPlayer = true;
    }

}
