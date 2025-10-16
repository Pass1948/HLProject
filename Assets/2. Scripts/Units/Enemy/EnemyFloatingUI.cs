using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyFloatingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI attributeText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private BaseEnemy model;
    // Start is called before the first frame update

    private void OnEnable()
    {
        GameManager.Event.Subscribe(EventType.EnemyUIUpdate, SetData);  
    }

    private void OnDisable()
    {
        GameManager.Event.Unsubscribe(EventType.EnemyUIUpdate, SetData);
    }


    void Start()
    {
        hpSlider.maxValue = model.enemyModel.maxHealth;
        hpSlider.value = model.enemyModel.currentHealth;
        
        string attri = null;
        if(model.enemyModel.attri == EnemyAttribute.High)
        {
            attri = "H";
        }
        else if(model.enemyModel.attri == EnemyAttribute.Low)
        {
            attri = "L";
        }
        attributeText.text = attri;
        
        string rankStr;
        switch (model.enemyModel.rank)
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
                rankStr = $"{model.enemyModel.rank}";
                break;
        }
        rankText.text = rankStr;
    }

    public void Init(EnemyModel model)
    {
        this.model.enemyModel = model;
    }

    public void SetData()
    {
        string attri = null;
        if (model.enemyModel.attri == EnemyAttribute.High)
        {
            attri = "H";
        }
        else if (model.enemyModel.attri == EnemyAttribute.Low)
        {
            attri = "L";
        }
        attributeText.text = attri;
        hpSlider.value = model.enemyModel.currentHealth;
    }
}
