using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStat playerStat;
    public MovementController movementcontroller;
    public AttackController attackController;

    private void Awake()
    {
        GameManager.Character.Player = this;
        playerStat = GetComponent<PlayerStat>();
        movementcontroller = GetComponent<MovementController>();
        attackController = GetComponent<AttackController>();
    }
}
