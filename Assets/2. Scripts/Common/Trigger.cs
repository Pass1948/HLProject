using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public SceneType testScene;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.SceneLoadManager.LoadScene(testScene);
    }
}
