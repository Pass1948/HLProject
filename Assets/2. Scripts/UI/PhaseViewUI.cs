using UnityEngine;
using TMPro;
using System;

public class PhaseViewUI : MonoBehaviour
{
    private TurnBasedManager turn;
    [SerializeField] private TMP_Text phaseViewText;

    private string nowPhase; //마지막으로 표시한 페이즈 상태

    private void Awake()
    {
        if(!turn && GameManager.TurnBased != null)
        {
            turn = GameManager.TurnBased;
        }
    }

    private void Start()
    {
        Refresh(true); //시작시 1회 갱신
    }

    private void Update()
    {
        Refresh(false);
    }

    private void Refresh(bool force)
    {
        if(!phaseViewText)
        {
            return;
        }

        string current = (turn != null) ? turn.GetState() : "None";

        if (!string.IsNullOrEmpty(current))
        {
            //플레이어 턴
            if (current.IndexOf("Player", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                phaseViewText.text = "PlayTurn";
            }
            //적 턴
            else if (current.IndexOf("Enemy", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                phaseViewText.text = "EnemyTurn";
            }
        }

        nowPhase = current;
    }
}
