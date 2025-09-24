using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnEffectController : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Event.Subscribe(EventType.EnemyTurnEnd, HandleTurnEnd);
    }

    private void OnDisable()
    {
        GameManager.Event.Unsubscribe(EventType.EnemyTurnEnd, HandleTurnEnd);
    }

    private void HandleTurnEnd()
    {
        // 리스트에서 효과 실행
        foreach (var effect in GameManager.Map.turnEndEffects)
        {
            if (effect != null)
            {
                effect.OnTurnEndAction();
            }
        }
    }
}
