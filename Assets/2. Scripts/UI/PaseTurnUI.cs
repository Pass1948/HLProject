using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaseTurnUI : BaseUI
{
    [SerializeField] GameObject playerTurn;
    [SerializeField] GameObject enemyTurn;
    [SerializeField] GameObject Phase;

    private void OnEnable()
    {
        CheckState();
    }


    void CheckState()
    {

        if(GameManager.TurnBased.GetState()== "PlayerTurnState")
        {
            playerTurn.SetActive(true);
            enemyTurn.SetActive(false);
            Phase.SetActive(false);
        }
        else if(GameManager.TurnBased.GetState() == "EnemyTurnState")
        {
            playerTurn.SetActive(false);
            enemyTurn.SetActive(true);
            Phase.SetActive(false);
        }
        else
        {
            playerTurn.SetActive(false);
            enemyTurn.SetActive(false);
            Phase.SetActive(true);
        }
    }




}
