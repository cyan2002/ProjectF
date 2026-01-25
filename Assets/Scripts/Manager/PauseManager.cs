using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static bool IsPaused;

    [SerializeField] GameObject pauseMenuUI;

    void Awake()
    {
        PlayerInput.HandleEscape += HandlePause;
    }

    public void HandlePause()
    {
        if (IsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }
}