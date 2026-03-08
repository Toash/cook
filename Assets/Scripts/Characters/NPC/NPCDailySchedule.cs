using Assets.Scripts.World;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

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


            var startNode = PedestrianGraph.I.GetClosestNode(NPC.transform.position);
            var endNode = PedestrianGraph.I.GetClosestNode(loc.transform.position);

            var path = PedestrianPathfinder.FindPath(startNode, endNode);

            NPC.Movement.SetNodePath(path);

            // NPC.Movement.Agent.SetDestination(loc.transform.position);
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

        /// <summary>
        /// Pathfinds to next node based off of closest node to the NPC.
        /// </summary>
        void GoToNextPoint()
        {
            SidewalkNode start = PedestrianGraph.I.GetClosestNode(NPC.transform.position);

            if (start == null || start.Neighbors.Count == 0)
                return;

            SidewalkNode end = start.Neighbors[Random.Range(0, start.Neighbors.Count)];
            NPC.Movement.Agent.SetDestination(end.transform.position);
        }

        public override void OnUpdate(NPCBrain brain)
        {
            if (NPC.Schedule.CurrentActivity != null)
                return;


            if (AgentUtils.HasAgentReachedDestination(NPC.Movement.Agent))
            {
                GoToNextPoint();
            }
        }


        // #if UNITY_EDITOR
        //         void OnDrawGizmosSelected()
        //         {
        //             if (StartNode != null)
        //             {
        //                 Handles.Label(StartNode.transform.position, "Start node");

        //             }
        //             if (EndNode != null)
        //             {
        //                 Handles.Label(EndNode.transform.position, "End node");

        //             }

        //         }
        // #endif

    }
}