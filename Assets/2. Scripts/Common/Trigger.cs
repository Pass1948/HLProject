using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public SceneType scene;

    void Start()
    {
        GameManager.SceneLoad.LoadScene(scene);
    }
}
