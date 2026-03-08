using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public int StartTimeSeconds = 18000;
    public float TimeSpeed;
    public float TimeScale;

    /// <summary>
    /// Time from 0 to 24th hour.
    /// </summary>
    [ShowInInspector, ReadOnly]
    private int timeInSeconds;
    float timer = 0;


    private const int END_OF_DAY_TIME = 86400;
    public static TimeManager I;

    /// <summary>
    /// Invokes every minute. Sends hour and minute
    /// </summary>
    public event Action<int, int> MinuteChanged;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(this);
            return;
        }
        else if (I == null)
        {
            DontDestroyOnLoad(gameObject);
            I = this;
        }
    }
    void Start()
    {
        StartDay();
    }

    public void StartDay()
    {
        timeInSeconds = StartTimeSeconds;
    }

    public int GetHour()
    {
        return timeInSeconds / 3600;
    }

    public int GetMinuteHours()
    {
        var minutes = timeInSeconds / 60;

        return minutes % 60;
    }

    public string Get24HourTime()
    {
        return GetHour().ToString("D2") + ":" + GetMinuteHours().ToString("D2");

    }

    public float GetNormalizedTime()
    {
        return (float)timeInSeconds / END_OF_DAY_TIME;
    }

    //[ConsoleMethod("SetTimeScale", "Sets the time scale.")]
    //public static void SetTimeScale(float scale)
    //{
    //    I.TimeScale = scale;
    //}

    void Update()
    {
        timer += Time.deltaTime * TimeSpeed * TimeScale;

        while (timer >= 1f)
        {
            timer -= 1f;
            timeInSeconds++;

            if (timeInSeconds % 3600 == 0)
            {
                Debug.Log("[TimeManager]: Hour changed, " + GetHour());
                //HourEntered?.Invoke(GetHour());
                MinuteChanged?.Invoke(GetHour(), GetMinuteHours());
            }

            if (timeInSeconds >= END_OF_DAY_TIME)
            {
                timeInSeconds = END_OF_DAY_TIME - 1;
                timer = 0f;
                break;
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
        string message = "Current Time: " + Get24HourTime() + "\n";
        message += "Time scale: " + TimeScale + "\n";
        message += "Normalized time: " + GetNormalizedTime().ToString("F2");
        Handles.Label(transform.position, message, style);


    }
#endif
}
