using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public static bool IsPaused;

    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject optionsMenuUI;

    void Awake()
    {
        PlayerInput.HandleEscape += HandlePause;
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
        optionsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        AudioManager.Instance.ResumeAudio();
        IsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        AudioManager.Instance.PauseAudio();
        IsPaused = true;
    }
}