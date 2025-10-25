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
     private EnemyModel model;

     private Quaternion fixedRotation;
     
    private void OnEnable()
    {
        GameManager.Event.Subscribe(EventType.EnemyUIUpdate, SetData);
        fixedRotation = transform.rotation;
    }

    private void OnDisable()
    {
        GameManager.Event.Unsubscribe(EventType.EnemyUIUpdate, SetData);
    }


    void Start()
    {
        if(model == null) Debug.Log("모델 없음");
        
        hpSlider.maxValue = model.maxHealth;
        hpSlider.value = model.currentHealth;
        
        string attri = null;
        if(model.attri == EnemyAttribute.High)
        {
            attri = "H";
        }
        else if(model.attri == EnemyAttribute.Low)
        {
            attri = "L";
        }
        attributeText.text = attri;
        
        string rankStr;
        switch (model.rank)
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
                rankStr = $"{model.rank}";
                break;
        }
        rankText.text = rankStr;
    }

    public void Init(EnemyModel model)
    {
        this.model = model;
    }

    public void SetData()
    {
        string attri = null;
        if (model.attri == EnemyAttribute.High)
        {
            attri = "H";
        }
        else if (model.attri == EnemyAttribute.Low)
        {
            attri = "L";
        }
        attributeText.text = attri;
        hpSlider.value = model.currentHealth;
    }
    
    private void LateUpdate()
    {
        transform.rotation = fixedRotation;
    }
}
