using UnityEngine;

namespace Assets.Scripts.Characters.NPC
{
    /// <summary>
    /// Ambient outside walking state.
    /// NPC walks sidewalk points and may decide to enter the restaurant.
    /// </summary>
    public class NPCSidewalkWander : NPCState
    {
        public override string Name => "NPCSidewalkWander";

        [Header("Wander")]
        [SerializeField] private float sidewalkNodeStopDistance = 1.5f;

        [Header("Restaurant Entry")]
        [SerializeField] private float enterRestaurantChance = 0.25f;
        [SerializeField] private float decisionCooldown = 2f;

        private float decisionTimer;

        public override void OnEnter(NPCBrain brain)
        {
            decisionTimer = 0f;
            NPC.Movement.Agent.stoppingDistance = sidewalkNodeStopDistance;

            if (!NPC.Movement.Agent.hasPath)
            {
                GoToNextPoint();
            }
        }

        public override void OnExit(NPCBrain brain)
        {
            NPC.Movement.Agent.stoppingDistance = NPC.Movement.DefaultStoppingDistance;
        }

        public override void OnFixedUpdate(NPCBrain brain)
        {
        }

        public override void OnUpdate(NPCBrain brain)
        {
            decisionTimer -= Time.deltaTime;

            if (ShouldEnterRestaurant())
            {
                Brain.ChangeState("NPCEnterRestaurant");
                return;
            }

            if (AgentUtils.HasAgentReachedDestination(NPC.Movement.Agent))
            {
                GoToNextPoint();
            }
        }

        private bool ShouldEnterRestaurant()
        {
            if (Brain.HasEatenToday)
                return false;

            if (decisionTimer > 0f)
                return false;

            decisionTimer = decisionCooldown;

            OrderStation station = FindBestOrderStation();
            if (station == null)
                return false;


            if (Random.value > enterRestaurantChance)
                return false;

            Brain.SetOrderStation(station);
            return true;
        }

        private OrderStation FindBestOrderStation()
        {
            OrderStation[] stations = Object.FindObjectsByType<OrderStation>(FindObjectsSortMode.None);

            if (stations == null || stations.Length == 0)
                return null;

            OrderStation closest = null;
            float closestDist = float.MaxValue;

            Vector3 pos = NPC.transform.position;

            foreach (var station in stations)
            {
                if (station == null) continue;

                float dist = Vector3.Distance(pos, station.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = station;
                }
            }

            return closest;
        }

        private void GoToNextPoint()
        {
            SidewalkNode start = PedestrianGraph.I.GetClosestNode(NPC.transform.position);

            if (start == null || start.Neighbors == null || start.Neighbors.Count == 0)
                return;

            SidewalkNode end = start.Neighbors[Random.Range(0, start.Neighbors.Count)];
            NPC.Movement.Agent.SetDestination(end.transform.position);
        }
    }
}