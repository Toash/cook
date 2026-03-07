using Assets.Scripts.World;
using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Characters.NPC
{
    /// <summary>
    /// Invokes events when entering and exiting activities.
    /// </summary>
    public class NPCSchedule : MonoBehaviour
    {
        public Schedule Schedule;

        [ShowInInspector, ReadOnly]
        public Activity CurrentActivity { get; private set; }
        public event Action<Activity> ActivityEnter;
        public event Action<Activity> ActivityExit;

        private void OnValidate()
        {
            if (Schedule.HasOverlaps())
            {
                Debug.LogError("NPC Schedule has overlaps!");
            }
        }
        void OnEnable()
        {
            TimeManager.I.MinuteChanged += OnMinuteChanged;
        }
        void OnDisable()
        {
            TimeManager.I.MinuteChanged -= OnMinuteChanged;
        }



        /// <summary>
        /// Detect for new activities 
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        void OnMinuteChanged(int hour, int minute)
        {
            Activity newActivity = null;

            foreach (var activity in Schedule.Activities)
            {
                if (activity.InTime(hour, minute))
                {
                    newActivity = activity;
                    break;
                }
            }

            if (newActivity == CurrentActivity)
                return;

            if (CurrentActivity != null)
            {
                var oldActivity = CurrentActivity;
                CurrentActivity = null;
                ActivityExit?.Invoke(oldActivity);
            }

            if (newActivity != null)
            {
                CurrentActivity = newActivity;
                ActivityEnter?.Invoke(CurrentActivity);
            }
        }


#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (CurrentActivity != null)
            {
                Handles.Label(transform.position, "Current Activity: " + CurrentActivity.ToString());
            }
        }
#endif

    }
}