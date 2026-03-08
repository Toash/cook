using Assets.Scripts.World;

namespace Assets.Scripts.Characters.NPC
{
    /// <summary>
    /// Default state <br/>
    /// State in which the NPC should respond to their daily schedule. <br
    /// </summary>
    public class NPCDailySchedule : NPCState
    {
        public override string StateName => "NPCDailySchedule";

        void OnActivityEnter(Activity activity)
        {
            Location loc = LocationManager.I.GetLocation(activity.LocationData);
            NPC.Movement.Agent.SetDestination(loc.transform.position);
        }
        void OnActivityExit(Activity activity)
        {
            NPC.Movement.Agent.ResetPath();

        }
        public override void OnEnter(NPCBrain brain)
        {
            // if we enter here from another state but are in an activity already.
            if (NPC.Schedule.CurrentActivity != null)
            {
                OnActivityEnter(NPC.Schedule.CurrentActivity);
            }

            Brain.NPC.Schedule.ActivityEnter += OnActivityEnter;
            Brain.NPC.Schedule.ActivityExit += OnActivityExit;
        }

        public override void OnExit(NPCBrain brain)
        {
            Brain.NPC.Schedule.ActivityEnter -= OnActivityEnter;
            Brain.NPC.Schedule.ActivityExit -= OnActivityExit;
        }

        public override void OnFixedUpdate(NPCBrain brain)
        {
        }

        public override void OnUpdate(NPCBrain brain)
        {

            // TODO, if no activity, just walk around
        }

    }
}