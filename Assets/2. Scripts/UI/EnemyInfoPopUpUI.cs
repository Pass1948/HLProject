using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyInfoPopUpUI : BaseUI
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI attributeText;
    [SerializeField] TextMeshProUGUI rankText;

    public string enemyName;
    public EnemyAttribute attribute;
    public int rank;

    protected override void OnOpen()
    {
        base.OnOpen();
        UpdateUI();
    }


    public void UpdateUI()
    {
        nameText.text = enemyName;

        string attri = null;
        if(attribute == EnemyAttribute.High)
        {
            attri = "High";
        }
        else if(attribute == EnemyAttribute.Low)
        {
            attri = "Low";
        }
        attributeText.text = attri;

        string rankStr;
        
        switch (rank)
        {
            case 1:
                rankStr = "A";
                break;
            case 11:
                rankStr = "J";
                break;
            case 12:
                rankStr = "Q";
                break;
            case 13:
                rankStr = "K";
                break;
            default:
                rankStr = $"{rank}";
                break;
        }

        attributeText.text = rankStr;
    }

}
