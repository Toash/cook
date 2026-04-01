using System.Text;
using TMPro;
using UnityEngine;

public class EndDaySummaryUI : MonoBehaviour
{

    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text summaryText;

    private DaySummary pendingSummary;
    public bool IsOpen => root.activeSelf;

    private void Awake()
    {
        root.SetActive(false);
    }

    private void Start()
    {
        DayManager.I.EndDaySummaryReady += OnEndDaySummaryReady;
    }

    private void OnDestroy()
    {
        if (DayManager.I != null)
            DayManager.I.EndDaySummaryReady -= OnEndDaySummaryReady;
    }

    private void OnEndDaySummaryReady(DaySummary summary)
    {
        pendingSummary = summary;
    }

    public void ShowSummary()
    {
        if (pendingSummary == null)
            return;

        root.SetActive(true);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Day {pendingSummary.DayNumber} Summary");
        sb.AppendLine();

        foreach (DaySummaryEntry entry in pendingSummary.Entries)
        {
            sb.AppendLine($"{entry.Label}: {entry.Value}");
        }

        summaryText.text = sb.ToString();
    }

    public void ConfirmEndDay()
    {
        root.SetActive(false);
        pendingSummary = null;
        DayManager.I.ConfirmEndDay();
    }
}