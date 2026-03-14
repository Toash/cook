
using Assets.Scripts.World;
using UnityEngine;

/// <summary>
/// Represents an activity that the npcs does at specific times.
/// </summary>
// [System.Serializable]
[CreateAssetMenu(fileName = "Activity", menuName = "NPC/Activity")]
public class Activity : ScriptableObject
{
    [Tooltip("The location of the activity.")]
    public LocationData LocationData;

    [Range(0, 23)] public int StartHour;
    [Range(0, 59)] public int StartMinute;

    [Range(0, 23)] public int EndHour;
    [Range(0, 59)] public int EndMinute;


    public override string ToString()
    {
        if (LocationData == null) return "No location data";
        return LocationData.Name;
    }

    public int StartTotalMinutes => StartHour * 60 + StartMinute;
    public int EndTotalMinutes => EndHour * 60 + EndMinute;

    public bool InTime(int hour, int minute)
    {
        bool inHour = hour >= StartHour && hour <= EndHour;
        bool inMinute = minute >= StartMinute && minute <= EndMinute;
        return inHour && inMinute;
    }


}