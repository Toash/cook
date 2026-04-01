using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayManager : MonoBehaviour
{
    public static DayManager I;

    [SerializeField] private int currentDay = 1;

    public int CurrentDay => currentDay;
    public bool IsAwaitingEndDayConfirmation => isAwaitingEndDayConfirmation;
    public DaySummary CurrentSummary => currentSummary;

    private bool isAwaitingEndDayConfirmation;
    private DaySummary currentSummary;
    private bool startDayAfterReload;

    public event Action<int> DayStarted;
    public event Action<int> DayEnded;
    public event Action<DaySummary> EndDaySummaryReady;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        TimeManager.I.ReachedEndOfDay += OnReachedEndOfDay;
        StartDay();
    }

    private void OnDestroy()
    {
        if (TimeManager.I != null)
            TimeManager.I.ReachedEndOfDay -= OnReachedEndOfDay;
    }

    public int GetDay()
    {
        return currentDay;
    }

    private void OnReachedEndOfDay()
    {
        if (isAwaitingEndDayConfirmation)
            return;

        isAwaitingEndDayConfirmation = true;
        currentSummary = BuildDaySummary();

        Debug.Log($"[DayManager]: Day {currentDay} reached end-of-day. Awaiting player confirmation.");

        EndDaySummaryReady?.Invoke(currentSummary);
    }

    public void ConfirmEndDay()
    {
        if (!isAwaitingEndDayConfirmation)
            return;

        Debug.Log($"[DayManager]: Ending Day {currentDay}");

        DayEnded?.Invoke(currentDay);

        isAwaitingEndDayConfirmation = false;
        currentSummary = null;

        StartNextDay();
    }

    public void StartNextDay()
    {
        currentDay++;

        Debug.Log($"[DayManager]: Starting Day {currentDay}");

        TimeManager.I.StartDay();
        startDayAfterReload = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void StartDay()
    {
        Debug.Log($"[DayManager]: Starting Day {currentDay}");

        TimeManager.I.StartDay();
        DayStarted?.Invoke(currentDay);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!startDayAfterReload)
            return;

        startDayAfterReload = false;
        DayStarted?.Invoke(currentDay);
    }

    private DaySummary BuildDaySummary()
    {
        DaySummary summary = new DaySummary();
        summary.DayNumber = currentDay;

        summary.Add("Day", currentDay);
        summary.Add("Time Ended", TimeManager.I.Get24HourTime());

        MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is IDaySummaryContributor contributor)
            {
                contributor.Contribute(summary);
            }
        }

        return summary;
    }
}