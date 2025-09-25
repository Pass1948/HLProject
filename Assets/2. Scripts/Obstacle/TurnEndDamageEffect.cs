using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnEndDamageEffect : MonoBehaviour, ITurnEndEffect
{
    [SerializeField] private int damageAmount = 1;
    private DamageAction _damageAction;

    private void Awake()
    {
        _damageAction = GetComponent<DamageAction>();
        if (_damageAction == null)
        {
            Debug.LogError("DamageAction 스크립트가 필요합니다!");
        }
    }
    
    private void OnEnable()
    {
        GameManager.Map.RegisterTurnEndEffect(this);
    }
    
    private void OnDisable()
    {
        if (GameManager.Map != null)
        {
            GameManager.Map.UnregisterTurnEndEffect(this);
        }
    }

    public void OnTurnEndAction()
    {
        if (_damageAction != null)
        {
            Vector3Int myCellPos = GameManager.Map.tilemap.WorldToCell(transform.position);
            _damageAction.ApplyDamage(myCellPos, damageAmount);
        }
    }
}
