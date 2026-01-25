using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public string gameSceneName = "GameScene"; // The main game scene

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game"); // Only visible in editor
        Application.Quit(); // Works in build
    }
}