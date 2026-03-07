using Assets.Scripts.World;

namespace Assets.Scripts.Characters.NPC
{
    /// <summary>
    /// State in which the NPC should respond to their daily schedule.
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
            NPC.Movement.Agent.isStopped = true;

        }
        public override void OnEnter(NPCBrain brain)
        {
            Brain.NPC.Schedule.ActivityEnter += OnActivityEnter;
            Brain.NPC.Schedule.ActivityEnter += OnActivityExit;
        }

        public override void OnExit(NPCBrain brain)
        {
            Brain.NPC.Schedule.ActivityEnter -= OnActivityEnter;
            Brain.NPC.Schedule.ActivityEnter -= OnActivityExit;
        }

        public override void OnFixedUpdate(NPCBrain brain)
        {
        }

        public override void OnUpdate(NPCBrain brain)
        {
        }

    }
}