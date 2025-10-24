using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInUI : BaseUI 
{
    public void OnCloseUI()
    {
        GameManager.UI.CloseUI<FadeInUI>();
    }
}
