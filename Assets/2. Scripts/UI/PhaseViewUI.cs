using UnityEngine;
using TMPro;

public class PhaseViewUI : MonoBehaviour
{
    [SerializeField] private TurnBasedManager turn;
    [SerializeField] private TMP_Text phaseViewText;

    private string nowPhase;

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

        if(force || current != nowPhase)
        {
            phaseViewText.text = ($"{current}");
        }
    }
}
