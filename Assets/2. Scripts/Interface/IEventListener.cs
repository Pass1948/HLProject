using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventListener
{
    //이벤트 발생시 전송되는 이벤트 정보.
    public void OnEvent(EventType eventType, Component Sender, object Param = null);
}
