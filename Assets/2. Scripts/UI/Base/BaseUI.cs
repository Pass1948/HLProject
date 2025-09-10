using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUI : MonoBehaviour
{
    public virtual void OpenUI()
    {
        gameObject.SetActive(true);
        OnOpen();
    }

    public virtual void CloseUI()
    {
        OnClose();
        gameObject.SetActive(false);
    }

    protected virtual void OnOpen() { }
    protected virtual void OnClose() { }
}
