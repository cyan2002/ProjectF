using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadUI : MonoBehaviour
{
    [SerializeField] GameObject saveButton;
    [SerializeField] GameObject loadButton;
    [SerializeField] bool isTitleScreen = false;

    void OnEnable()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        if (saveButton != null)
            saveButton.SetActive(!isTitleScreen);

        if (loadButton != null && SaveManager.Instance != null)
            loadButton.SetActive(SaveManager.Instance.SaveExists());
    }

    public void OnSavePressed()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector2 playerPos = player != null ? (Vector2)player.transform.position : Vector2.zero;

        int money = MoneyManager.Instance.Money;
        float time = Clock.Instance.timeOfDay;
        int day = Clock.Instance.dayNumber;

        SaveManager.Instance.SaveWorldState(playerPos, MoneyManager.Instance.Money, time, day);
        SaveManager.Instance.SaveAllGrids();

        OnEnable();
        Debug.Log("Game saved.");
    }

    public void OnLoadPressed()
    {
        PauseManager.Instance.Resume();
        if (!SaveManager.Instance.SaveExists()) return;

        WorldSaveData world = SaveManager.Instance.LoadWorldState();

        if (!string.IsNullOrEmpty(world.sceneName))
        {
            string sceneToUnload = "";
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (s.name != "Master" && s.name != world.sceneName)
                {
                    sceneToUnload = s.name;
                    break;
                }
            }

            SceneLoader.Instance.TransitionToSceneFromSave(world.sceneName, sceneToUnload);
        }
    }

    public void OnQuitPressed()
    {
        SaveManager.Instance.DeleteSave();
        OnEnable();
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}