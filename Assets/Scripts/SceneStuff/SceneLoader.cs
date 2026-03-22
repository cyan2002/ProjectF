using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    [SerializeField] private string startingScene;
    public GameObject fadePanel;
    public bool isTransitioning = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (SceneManager.sceneCount <= 1)
        {
            string sceneToLoad = startingScene;

#if UNITY_EDITOR
            string devScene = UnityEditor.EditorPrefs.GetString("DevStartScene", "");
            if (!string.IsNullOrEmpty(devScene))
                sceneToLoad = System.IO.Path.GetFileNameWithoutExtension(devScene);
#endif
            StartCoroutine(LoadStartingScene(sceneToLoad));
        }
    }

    IEnumerator LoadStartingScene(string sceneName)
    {
        fadePanel.SetActive(true);
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return null;
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

    public void TransitionToScene(string loadScene, string unloadScene, string spawnID)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        loadScene = loadScene.Trim();
        unloadScene = unloadScene.Trim();
        spawnID = spawnID.Trim();

        StartCoroutine(TransitionCoroutine(loadScene, unloadScene, spawnID));
    }

    // Separate transition used when loading from save
    public void TransitionToSceneFromSave(string loadScene, string unloadScene)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        StartCoroutine(LoadFromSaveCoroutine(loadScene, unloadScene));
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

        // Unload old scene
        Scene sceneToUnloadRef = default;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == unloadScene)
            {
                sceneToUnloadRef = SceneManager.GetSceneAt(i);
                break;
            }
        }

        if (sceneToUnloadRef.IsValid() && sceneToUnloadRef.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(sceneToUnloadRef);
            yield return null;
        }

        // Load new scene
        yield return SceneManager.LoadSceneAsync(loadScene, LoadSceneMode.Additive);
        yield return null;

        // Move player to spawn point — skip if loading from save
        if (!SaveManager.Instance.isLoadingFromSave)
        {
            SpawnPoint[] spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
            SpawnPoint spawn = System.Array.Find(spawnPoints, s => s.spawnID == spawnID);
            if (spawn != null)
                GameObject.FindWithTag("Player").transform.position = spawn.transform.position;
            else
                Debug.LogWarning("SpawnPoint not found: " + spawnID);
        }

        // Fade in
        while (cg.alpha > 0f)
        {
            cg.alpha -= Time.deltaTime;
            yield return null;
        }

        fadePanel.SetActive(false);
        isTransitioning = false;
    }

    IEnumerator LoadFromSaveCoroutine(string loadScene, string unloadScene)
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

        // Unload current scene
        Scene sceneToUnloadRef = default;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == unloadScene)
            {
                sceneToUnloadRef = SceneManager.GetSceneAt(i);
                break;
            }
        }

        if (sceneToUnloadRef.IsValid() && sceneToUnloadRef.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(sceneToUnloadRef);
            yield return null;
        }

        // Set flag AFTER unload, BEFORE load
        // OnSceneLoaded will now see isLoadingFromSave as true for the new scene
        SaveManager.Instance.isLoadingFromSave = true;

        // Load saved scene
        yield return SceneManager.LoadSceneAsync(loadScene, LoadSceneMode.Additive);
        yield return null;

        // Fade in
        while (cg.alpha > 0f)
        {
            cg.alpha -= Time.deltaTime;
            yield return null;
        }

        fadePanel.SetActive(false);
        isTransitioning = false;
    }
}