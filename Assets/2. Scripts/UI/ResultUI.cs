using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResultType
{
    Clear,
    Over
}

public class ResultUI : BaseUI
{

    [SerializeField] ClearUI clearUI;
    [SerializeField] OverUI overUI;

    public ResultType resulttype;

    private void OnEnable()
    {
        
    }

    public void GetResultType(ResultType result)
    {
        Debug.Log("GetResultType");

        if (result == ResultType.Clear)
        {
            overUI.CloseUI();
            clearUI.OpenUI();
        }
        else if (result == ResultType.Over)
        {
            clearUI.CloseUI();
            overUI.OpenUI();
        }
    }


}
