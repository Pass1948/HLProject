using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{
    public PlayerModel playerModel;
    public MovementController controller;
    public PlayerAnimHandler animHandler;
    public PlayerHandler playerHandler;

    protected virtual void Awake()
    {
        GameManager.Unit.Player = this;
        playerModel = new PlayerModel();
        animHandler = GetComponent<PlayerAnimHandler>();
        controller = GetComponent<MovementController>();
        playerHandler = GetComponent<PlayerHandler>();
        controller.isPlayer = true;
    }

    protected virtual void Start()
    {

    }
}
