using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStat playerStat;
    public CharacterMovementController movementcontroller;
    public AttackTestController attackController;

    private void Awake()
    {
        GameManager.Character.Player = this;
        playerStat = GetComponent<PlayerStat>();
        movementcontroller = GetComponent<CharacterMovementController>();
        attackController = GetComponent<AttackTestController>();
    }
}
