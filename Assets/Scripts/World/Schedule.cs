using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.World
{
    /// <summary>
    /// Represents an activity that the npcs does at specific times.
    /// </summary>
    [System.Serializable]
    public class Activity
    {
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


    [System.Serializable]
    public class Schedule
    {
        // activity times should not overlap!
        public List<Activity> Activities;


        public bool HasOverlaps()
        {
            for (int i = 0; i < Activities.Count; i++)
            {
                var a = Activities[i];
                if (a == null) continue;

                if (a.EndTotalMinutes <= a.StartTotalMinutes)
                    return true;

                for (int j = i + 1; j < Activities.Count; j++)
                {
                    var b = Activities[j];
                    if (b == null) continue;

                    bool overlap =
                        a.StartTotalMinutes < b.EndTotalMinutes &&
                        b.StartTotalMinutes < a.EndTotalMinutes;

                    if (overlap)
                        return true;
                }
            }

            return false;
        }
    }
}