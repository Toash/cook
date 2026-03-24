using System;
using UnityEngine;

/// <summary>
/// Controls day progression (player-triggered).
/// </summary>
public class DayManager : MonoBehaviour
{
    public static DayManager I;

    [SerializeField] private int currentDay = 1;

    public event Action<int> DayStarted;
    public event Action<int> DayEnded;

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

    public int GetDay()
    {
        return currentDay;
    }

    public void EndDay()
    {
        Debug.Log($"[DayManager]: Ending Day {currentDay}");

        DayEnded?.Invoke(currentDay);
    }

    public void StartNextDay()
    {
        currentDay++;

        Debug.Log($"[DayManager]: Starting Day {currentDay}");

        TimeManager.I.StartDay();

        DayStarted?.Invoke(currentDay);
    }

    private void StartDay()
    {
        Debug.Log($"[DayManager]: Starting Day {currentDay}");

        TimeManager.I.StartDay();

        DayStarted?.Invoke(currentDay);
    }
}