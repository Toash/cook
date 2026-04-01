using System.Collections.Generic;

public class DaySummary
{
    public int DayNumber;
    public readonly List<DaySummaryEntry> Entries = new();

    public void Add(string label, string value)
    {
        Entries.Add(new DaySummaryEntry(label, value));
    }

    public void Add(string label, int value)
    {
        Add(label, value.ToString());
    }

    public void Add(string label, float value, string format = "F0")
    {
        Add(label, value.ToString(format));
    }
}

public readonly struct DaySummaryEntry
{
    public readonly string Label;
    public readonly string Value;

    public DaySummaryEntry(string label, string value)
    {
        Label = label;
        Value = value;
    }
}