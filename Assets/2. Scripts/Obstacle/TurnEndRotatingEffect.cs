using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnEndRotatingEffect : MonoBehaviour, ITurnEndEffect
{
    private RotationAction _rotationAction;

    private void Awake()
    {
        _rotationAction = GetComponent<RotationAction>();
        if (_rotationAction == null)
        {
            Debug.LogError("RotationAction 스크립트가 필요합니다!");
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
        if (_rotationAction != null)
        {
            Vector3Int myCellPos = GameManager.Map.tilemap.WorldToCell(transform.position);
            _rotationAction.RotateUnits(myCellPos);
        }
    }
}
