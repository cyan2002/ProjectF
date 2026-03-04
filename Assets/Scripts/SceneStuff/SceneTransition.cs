using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Street";
    [SerializeField] private string sceneToUnload = "Shop";
    [SerializeField] private string spawnPointID = "from_shop";

    private bool inRange = false;

    private void Start()
    {
        PlayerInput.HandleE += pressE;
    }

    private void pressE()
    {
        if (inRange)
        {
            SceneLoader.Instance.TransitionToScene(sceneToLoad, sceneToUnload, spawnPointID);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            inRange = false;
        }
    }
}
