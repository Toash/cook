using Assets.Scripts.Vehicle;
using UnityEngine;

namespace Assets.Scripts.Characters.NPC
{
    public class NPCWanderWaypoints : NPCState
    {
        public override string Name => "NPCWanderWaypoints";

        public Transform[] Waypoints;

        private int currentIndex = 0;

        Transform getNextWaypoint()
        {
            var ret = Waypoints[currentIndex++];
            if (currentIndex == Waypoints.Length)
            {
                currentIndex = 0;
            }
            return ret;
        }
        void OnFoodTruckEntered(FoodTruck truck)
        {
            Brain.ChangeState("NPCWaitInLine");

        }
        public override void OnEnter(NPCBrain brain)
        {
            Brain.EnteredFoodTruck += OnFoodTruckEntered;
        }

        public override void OnExit(NPCBrain brain)
        {
            Brain.EnteredFoodTruck -= OnFoodTruckEntered;
        }

        public override void OnFixedUpdate(NPCBrain brain)
        {
        }

        public override void OnUpdate(NPCBrain brain)
        {
            if (AgentUtils.HasAgentReachedDestination(Brain.NPC.Movement.Agent))
            {
                var waypoint = getNextWaypoint();
                Brain.NPC.Movement.Agent.SetDestination(waypoint.position);
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            foreach (var waypoint in Waypoints)
            {
                Gizmos.DrawSphere(waypoint.position, .2f);
            }
        }
#endif
    }
}