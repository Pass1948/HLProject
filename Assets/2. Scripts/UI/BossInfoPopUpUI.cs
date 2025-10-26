using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossInfoPopUpUI : BaseUI
{

    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI moveRangeText;
    [SerializeField] private TextMeshProUGUI attributeText;
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private Image hpBar;
    [SerializeField] private Button infoButton;
    [SerializeField] private Image infoPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI remainCooldownText;

    private bool infoPanelOpen = false;
    
    private EnemyAttribute attribute;
    private int rank;
    private int attack;
    private int moveRange;
    private int maxHealth;
    private int currentHealth;
    
    protected override void OnOpen()
    {
        base.OnOpen();
        infoButton.onClick.AddListener(ToggleInfoPanel);
        UpdateUI();                                      // UI가 열려있으면 즉시 갱신
    }

    private void ToggleInfoPanel()
    {
        infoPanelOpen = !infoPanelOpen;
        InfoPanelControl(infoPanelOpen);
    }

    private void InfoPanelControl(bool open)
    {
        infoPanel.gameObject.SetActive(open);
    }

    public void SetData(EnemyAttribute attr, int rank, int attack, int moveRange, int currentHp, int maxHp)
    {
        attribute = attr;
        this.rank = rank;
        this.attack = attack;
        this.moveRange = moveRange;
        maxHealth = maxHp;
        currentHealth = currentHp;
        
        UpdateUI();
    }

    public void UpdateUI()
    {
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

        rankText.text = rankStr;
        attackText.text = attack.ToString();
        moveRangeText.text = moveRange.ToString();
        hpBar.fillAmount = currentHealth / (float)maxHealth;
    }
}
