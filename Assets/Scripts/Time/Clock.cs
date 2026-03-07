using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

public class Clock : MonoBehaviour
{
    public static Clock Instance { get; private set; }
    public static event Action<int> OnHourChanged;
    public static event Action<int> OnDayChanged;
    
    private void Awake()
    {
        Instance = this;
        clockText = this.gameObject.GetComponent<TextMeshProUGUI>();
    }

    public float dayLengthInMinutes = 10f;
    public int startHour = 8;

    public float timeOfDay;   // 0 → 24
    public int dayNumber = 1;

    public TextMeshProUGUI clockText;

    int lastHour, lastMinute;

    public int Hour => Mathf.FloorToInt(timeOfDay);
    public int Minute => Mathf.FloorToInt((timeOfDay % 1f) * 60f);

    void Start()
    {
        timeOfDay = startHour;
    }

    void Update()
    {
        float secondsPerGameDay = dayLengthInMinutes * 60f;
        float timeScale = 24f / secondsPerGameDay;

        timeOfDay += Time.deltaTime * timeScale;

        if (timeOfDay >= 24f)
        {
            timeOfDay -= 24f;
            dayNumber++;
            OnDayChanged?.Invoke(dayNumber);
        }

        int currentHour = Hour;
        int currentMinute = Minute;

        if (currentHour != lastHour)
        {
            lastHour = currentHour;
            OnHourChanged?.Invoke(currentHour);
        }

        if (currentMinute != lastMinute)
        {
            lastMinute = currentMinute;
            UpdateClockUI();
        }
    }

    void UpdateClockUI()
    {
        if (clockText == null) return;
        clockText.text = $"Day {dayNumber} — {Hour:00}:{Minute:00}";
    }
}
