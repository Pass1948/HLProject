using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearUI : BaseUI
{
    public void OnClickTitle()
    {
        GameManager.SceneLoad.LoadScene(SceneType.Title);
    }
}
