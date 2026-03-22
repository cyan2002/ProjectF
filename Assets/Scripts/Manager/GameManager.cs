using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//handles goal
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject WinScreen;

    [SerializeField] public int goalDeadlineDay = 7;
    [SerializeField] private float moneyGoal = 20000f;
    public bool goalCompleted = false;

    [SerializeField] private TextMeshProUGUI goalText;


    private void OnEnable()
    {
        Clock.OnDayChanged += HandleTimeChanged;
    }

    private void OnDisable()
    {
        Clock.OnDayChanged -= HandleTimeChanged;
    }

    private void Awake()
    {
        WinScreen.SetActive(false);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // prevent duplicates
        }
    }

    private void Start()
    {
        UpdateGoalUI();
    }

    private void HandleTimeChanged(int day)
    {
        if (day == goalDeadlineDay)
        {
            if (MoneyManager.Instance.Money > moneyGoal)
            {
                goalCompleted = true;
                PlayWinScreen();
            }
            else
            {
                goalCompleted = false;
            }
        }
        else
        {
            if (MoneyManager.Instance.Money > moneyGoal)
            {
                goalCompleted = true;
                PlayWinScreen();
            }
        }
    }

    public void UpdateGoalUI()
    {
        if (goalText == null) return;

        float earned = MoneyManager.Instance.MoneyEarned;
        goalText.text = $"Goal: ${earned:F2} / ${moneyGoal:F2}";
    }

    //temp win condition
    private void PlayWinScreen()
    {
        WinScreen.SetActive(true);
    }
}
