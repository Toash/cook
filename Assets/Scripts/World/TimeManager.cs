using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public int StartTimeSeconds = 18000; // 5:00 AM
    public float TimeSpeed = 60f;
    public float TimeScale = 1f;
    public int CurrentTimeSeconds => timeInSeconds;

    /// <summary>
    /// Time within current day [0, 86399]
    /// </summary>
    [ShowInInspector, ReadOnly]
    private int timeInSeconds;

    private float timer = 0f;

    private const int SecondsPerMinute = 60;
    private const int SecondsPerHour = 3600;
    private const int SecondsPerDay = 86400;

    private bool hasReachedEndOfDay = false;

    public static TimeManager I;

    /// <summary>
    /// Fires when minute changes (hour, minute)
    /// </summary>
    public event Action<int, int> MinuteChanged;

    /// <summary>
    /// Fires once when day reaches end (23:59:59)
    /// </summary>
    public event Action ReachedEndOfDay;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartDay();
    }

    public void StartDay()
    {
        timeInSeconds = StartTimeSeconds;
        timer = 0f;
        hasReachedEndOfDay = false;

        Debug.Log($"[TimeManager]: Day started at {Get24HourTime()}");

        MinuteChanged?.Invoke(GetHour(), GetMinute());
    }

    public void SetTime(int hour, int minute)
    {
        hour = Mathf.Clamp(hour, 0, 23);
        minute = Mathf.Clamp(minute, 0, 59);

        timeInSeconds = hour * SecondsPerHour + minute * SecondsPerMinute;
        timer = 0f;

        Debug.Log($"[TimeManager]: Time set to {Get24HourTime()}");

        MinuteChanged?.Invoke(GetHour(), GetMinute());
    }

    public int GetHour()
    {
        return timeInSeconds / SecondsPerHour;
    }

    public int GetMinute()
    {
        return (timeInSeconds / SecondsPerMinute) % 60;
    }

    public int GetTotalMinutes()
    {
        return timeInSeconds / SecondsPerMinute;
    }

    public string Get24HourTime()
    {
        return $"{GetHour():D2}:{GetMinute():D2}";
    }

    public float GetNormalizedTime()
    {
        return (float)timeInSeconds / SecondsPerDay;
    }

    public bool IsEndOfDay()
    {
        return hasReachedEndOfDay;
    }

    void Update()
    {
        if (hasReachedEndOfDay)
            return;

        timer += Time.deltaTime * TimeSpeed * TimeScale;

        while (timer >= 1f)
        {
            timer -= 1f;

            int previousMinute = GetTotalMinutes();

            timeInSeconds++;

            if (timeInSeconds >= SecondsPerDay)
            {
                timeInSeconds = SecondsPerDay - 1;
                timer = 0f;

                if (!hasReachedEndOfDay)
                {
                    hasReachedEndOfDay = true;
                    Debug.Log("[TimeManager]: Reached end of day");
                    ReachedEndOfDay?.Invoke();
                }

                return;
            }

            int newMinute = GetTotalMinutes();

            if (newMinute != previousMinute)
            {
                MinuteChanged?.Invoke(GetHour(), GetMinute());
            }
        }
    }

#if UNITY_EDITOR
    GUIStyle style = new();

    private void OnDrawGizmosSelected()
    {
        style.normal.textColor = Color.blue;
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 24;

        string message = $"Time: {Get24HourTime()}\n";
        message += $"TimeScale: {TimeScale}\n";
        message += $"Normalized: {GetNormalizedTime():F2}";

        Handles.Label(transform.position, message, style);
    }
#endif
}