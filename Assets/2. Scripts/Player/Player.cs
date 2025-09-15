using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public UnitModel model;
    public PlayerStat playerStat;
    public MovementController movementcontroller;
    public AttackController attackController;

    private void Awake()
    {
        GameManager.Unit.Player = this;
        playerStat = GetComponent<PlayerStat>();
        movementcontroller = GetComponent<MovementController>();
        attackController = GetComponent<AttackController>();
    }
}
