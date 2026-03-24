using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.World
{
    [CreateAssetMenu(fileName = "Schedule", menuName = "NPC/Schedule")]
    public class Schedule : ScriptableObject
    {
        [InlineEditor]
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

        public Activity GetCurrentActivity(int hour, int minute)
        {
            if (Activities == null) return null;

            foreach (var activity in Activities)
            {
                if (activity != null && activity.InTime(hour, minute))
                    return activity;
            }

            return null;
        }
    }
}