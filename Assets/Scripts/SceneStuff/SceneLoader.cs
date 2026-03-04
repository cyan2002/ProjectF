using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

//script helps load scenes from the title screen to play
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    [SerializeField] private string startingScene = "Shop";
    public GameObject fadePanel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (SceneManager.sceneCount <= 1)
        {
            string devScene = PlayerPrefs.GetString("DevStartScene", "");
            string sceneToLoad = !string.IsNullOrEmpty(devScene)
                ? System.IO.Path.GetFileNameWithoutExtension(devScene)
                : startingScene;
            StartCoroutine(LoadStartingScene(sceneToLoad));
        }
    }

    // Initial load
    IEnumerator LoadStartingScene(string sceneName)
    {
        fadePanel.SetActive(true);
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        CanvasGroup cg = fadePanel.GetComponent<CanvasGroup>();
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            cg.alpha = 1f - timer;
            yield return null;
        }
        fadePanel.SetActive(false);
    }

    // Scene transitions
    public void TransitionToScene(string loadScene, string unloadScene, string spawnID)
    {
        StartCoroutine(TransitionCoroutine(loadScene, unloadScene, spawnID));
    }

    IEnumerator TransitionCoroutine(string loadScene, string unloadScene, string spawnID)
    {
        // Fade out
        fadePanel.SetActive(true);
        CanvasGroup cg = fadePanel.GetComponent<CanvasGroup>();
        cg.alpha = 0f;
        while (cg.alpha < 1f)
        {
            cg.alpha += Time.deltaTime;
            yield return null;
        }

        // Load new scene
        yield return SceneManager.LoadSceneAsync(loadScene, LoadSceneMode.Additive);

        // Move player to spawn point
        SpawnPoint[] spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        SpawnPoint spawn = System.Array.Find(spawnPoints, s => s.spawnID == spawnID);
        if (spawn != null)
            GameObject.FindWithTag("Player").transform.position = spawn.transform.position;

        // Unload old scene
        yield return SceneManager.UnloadSceneAsync(unloadScene);

        // Fade in
        while (cg.alpha > 0f)
        {
            cg.alpha -= Time.deltaTime;
            yield return null;
        }
        fadePanel.SetActive(false);
    }
}

public class SpawnPoint : MonoBehaviour
{
    public string spawnID; // e.g. "from_shop", "from_house"
}
