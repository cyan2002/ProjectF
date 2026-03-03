using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string startingScene = "Shop";

    void Start()
    {
        if (SceneManager.sceneCount <= 1)
        {
            SceneManager.LoadSceneAsync(startingScene, LoadSceneMode.Additive);
        }
    }
}
