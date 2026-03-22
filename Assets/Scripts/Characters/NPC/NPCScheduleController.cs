using Assets.Scripts.World;
using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Characters.NPC
{
    /// <summary>
    /// Invokes events when time to enter and exit activities.
    /// </summary>
    public class NPCScheduleController : MonoBehaviour
    {
        // the schedule that the npc has.
        public Schedule Schedule;

        [ShowInInspector, ReadOnly]
        public Activity CurrentActivity { get; private set; }
        public event Action<Activity> ActivityEnter;
        public event Action<Activity> ActivityExit;

        void Start()
        {
            // if (Schedule == null) return;

            // load random schedule

            Schedule schedule = Resources.LoadAll<Schedule>("ScriptableObjects/Schedules")[UnityEngine.Random.Range(0, Resources.LoadAll<Schedule>("ScriptableObjects/Schedules").Length)];
            Schedule = schedule;


            if (Schedule.HasOverlaps())
            {
                Debug.LogError("NPC Schedule has overlaps!");
            }
            TimeManager.I.MinuteChanged += OnMinuteChanged;
            ActivityEnter += OnActivityEnter;
            ActivityExit += OnActivityExit;
        }
        void OnDestroy()
        {
            if (Schedule == null) return;
            TimeManager.I.MinuteChanged -= OnMinuteChanged;
            ActivityEnter -= OnActivityEnter;
            ActivityExit -= OnActivityExit;
        }


        void OnActivityEnter(Activity activity)
        {
            Debug.Log("[NPCSchedule]: Entered activity " + activity);

        }
        void OnActivityExit(Activity activity)
        {
            Debug.Log("[NPCSchedule]: Exited activity " + activity);

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