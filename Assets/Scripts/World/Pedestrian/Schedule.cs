using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.World
{


    /// <summary>
    /// Represents a list of activites
    /// </summary>
    // [System.Serializable]
    [CreateAssetMenu(fileName = "Schedule", menuName = "NPC/Schedule")]
    public class Schedule : ScriptableObject
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