using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScene : MonoBehaviour
{
    public abstract void SceneLoading();
    public abstract void SceneEnter();
    public abstract void SceneExit();
}
