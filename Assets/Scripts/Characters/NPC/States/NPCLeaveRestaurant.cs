using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Characters.NPC
{
    /// <summary>
    /// NPC leaves the restaurant, then either returns to sidewalk wandering or despawns.
    /// </summary>
    public class NPCLeaveRestaurant : NPCState
    {
        public override string Name => "NPCLeaveRestaurant";


        private Coroutine leaveRoutine;

        public override void OnEnter(NPCBrain brain)
        {
            if (Brain.CurrentOrderStation != null && Brain.CurrentOrderStation.OrderLine != null)
            {
                Brain.CurrentOrderStation.OrderLine.RemoveNPCFromLine(Brain);
            }

            leaveRoutine = StartCoroutine(LeaveRestaurantCoroutine());
        }

        public override void OnExit(NPCBrain brain)
        {
            if (leaveRoutine != null)
            {
                StopCoroutine(leaveRoutine);
                leaveRoutine = null;
            }
        }

        public override void OnFixedUpdate(NPCBrain brain)
        {
        }

        public override void OnUpdate(NPCBrain brain)
        {
        }

        private IEnumerator LeaveRestaurantCoroutine()
        {
            Vector3 exitPoint = GetExitPoint();

            yield return NPC.Movement.MoveAndAwait(exitPoint);

            Brain.ClearOrderStation();

            leaveRoutine = null;

            NPCSpawnerManager.I.UnregisterNPC(Brain);
            Brain.ChangeState("NPCSidewalkWander");
        }

        private Vector3 GetExitPoint()
        {
            if (Brain.CurrentOrderStation != null)
            {
                return Brain.CurrentOrderStation.ExitSpot.position;
            }

            return NPC.transform.position;
        }
    }
}