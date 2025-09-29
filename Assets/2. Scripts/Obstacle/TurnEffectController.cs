using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnEffectController : MonoBehaviour
{
    private void OnEnable()
    {
        // 턴 종료 이벤트를 구독
        GameManager.Event.Subscribe(EventType.EnemyTurnEnd, HandleTurnEnd);
    }

    private void OnDisable()
    {
        // 오브젝트가 비활성화되면 구독을 해제
        GameManager.Event.Unsubscribe(EventType.EnemyTurnEnd, HandleTurnEnd);
    }

    private void HandleTurnEnd()
    {
        foreach (var effect in GameManager.Map.turnEndEffects.ToArray())
        {
            if (effect != null)
            {
                effect.OnTurnEndAction();
            }
        }
    }
}
