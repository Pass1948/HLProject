using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutUI : BaseUI
{
    public void OnCloseUI()
    {
        GameManager.UI.CloseUI<FadeOutUI>();
    }
}
