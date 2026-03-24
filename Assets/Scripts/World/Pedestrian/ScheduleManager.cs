using System;
using System.Collections.Generic;
using Assets.Scripts.World;
using UnityEngine;

/// <summary>
/// Watches schedules and invokes events before activities begin.
/// Assumes time moves forward through a shared time source.
/// </summary>
public class ScheduleManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Schedules to monitor.")]
    [SerializeField, Sirenix.OdinInspector.InlineEditor] private List<Schedule> schedules = new();

    [Tooltip("How many in-game minutes before an activity starts should the warning event fire?")]
    [SerializeField] private int notifyMinutesBefore = 30;

    [Header("Debug")]
    [SerializeField] private bool logEvents = true;

    /// <summary>
    /// Fired when an activity is about to start.
    /// int = minutes remaining until start
    /// </summary>
    public event Action<Schedule, Activity, int> OnActivityStartingSoon;

    /// <summary>
    /// Fired exactly when an activity starts.
    /// </summary>
    public event Action<Schedule, Activity> OnActivityStarted;

    /// <summary>
    /// Fired exactly when an activity ends.
    /// </summary>
    public event Action<Schedule, Activity> OnActivityEnded;

    // Prevent duplicate firing for the same day/minute.
    private readonly HashSet<string> firedStartingSoon = new();
    private readonly HashSet<string> firedStarted = new();
    private readonly HashSet<string> firedEnded = new();

    private int lastProcessedDay = -1;

    void Update()
    {
        // Replace this with your real world time source.
        if (!TryGetCurrentTime(out int day, out int hour, out int minute))
            return;

        // Clear per-day event memory when a new day begins.
        if (day != lastProcessedDay)
        {
            firedStartingSoon.Clear();
            firedStarted.Clear();
            firedEnded.Clear();
            lastProcessedDay = day;
        }

        int nowTotalMinutes = hour * 60 + minute;

        foreach (var schedule in schedules)
        {
            if (schedule == null || schedule.Activities == null)
                continue;

            foreach (var activity in schedule.Activities)
            {
                if (activity == null)
                    continue;

                int start = activity.StartTotalMinutes;
                int end = activity.EndTotalMinutes;

                int minutesUntilStart = start - nowTotalMinutes;

                string soonKey = $"{day}_soon_{schedule.GetInstanceID()}_{activity.GetInstanceID()}";
                string startedKey = $"{day}_start_{schedule.GetInstanceID()}_{activity.GetInstanceID()}";
                string endedKey = $"{day}_end_{schedule.GetInstanceID()}_{activity.GetInstanceID()}";

                if (minutesUntilStart == notifyMinutesBefore && !firedStartingSoon.Contains(soonKey))
                {
                    firedStartingSoon.Add(soonKey);
                    OnActivityStartingSoon?.Invoke(schedule, activity, minutesUntilStart);

                    if (logEvents)
                        Debug.Log($"[ScheduleManager] Activity starting soon: {activity.name} at {FormatTime(start)}");
                }

                if (nowTotalMinutes == start && !firedStarted.Contains(startedKey))
                {
                    firedStarted.Add(startedKey);
                    OnActivityStarted?.Invoke(schedule, activity);

                    if (logEvents)
                        Debug.Log($"[ScheduleManager] Activity started: {activity.name}");
                }

                if (nowTotalMinutes == end && !firedEnded.Contains(endedKey))
                {
                    firedEnded.Add(endedKey);
                    OnActivityEnded?.Invoke(schedule, activity);

                    if (logEvents)
                        Debug.Log($"[ScheduleManager] Activity ended: {activity.name}");
                }
            }
        }
    }

    private string FormatTime(int totalMinutes)
    {
        int h = totalMinutes / 60;
        int m = totalMinutes % 60;
        return $"{h:00}:{m:00}";
    }

    /// <summary>
    /// Replace this with your actual game time system.
    /// </summary>
    private bool TryGetCurrentTime(out int day, out int hour, out int minute)
    {
        day = 0;
        hour = 0;
        minute = 0;

        if (TimeManager.I == null)
        {
            Debug.LogWarning("ScheduleManager: No TimeManager found.");
            return false;
        }
        hour = TimeManager.I.GetHour();
        minute = TimeManager.I.GetMinute();

        return true;
    }
}