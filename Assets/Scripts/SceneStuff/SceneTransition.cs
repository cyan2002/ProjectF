using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string sceneToUnload;
    [SerializeField] private string spawnPointID;

    private bool inRange = false;

    private void OnEnable()
    {
        PlayerInput.HandleE += pressE;
    }

    private void OnDisable()
    {
        PlayerInput.HandleE -= pressE;
    }

    private void pressE()
    {
        if (inRange && !SceneLoader.Instance.isTransitioning)
        {
            // Save grids to session memory only — not disk
            SaveManager.Instance.SaveGridsToSession();
            InventoryController.Instance.ResetHighlighterParent();
            SceneLoader.Instance.TransitionToScene(sceneToLoad, sceneToUnload, spawnPointID);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            inRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            inRange = false;
    }
}