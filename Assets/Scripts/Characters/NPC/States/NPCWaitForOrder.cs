using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Characters.NPC
{
    /// <summary>
    /// NPC has placed an order and waits at the station's wait spot
    /// until their submitted order is ready for pickup.
    /// If they wait too long, they cancel and leave.
    /// </summary>
    public class NPCWaitForOrder : NPCState
    {
        public override string Name => "NPCWaitForOrder";

        private float maxWaitSeconds = 6f;

        private Coroutine moveRoutine;
        private bool orderReady;
        private float waitTimer;

        private OrderStation Station => Brain.CurrentOrderStation;

        public override void OnEnter(NPCBrain brain)
        {
            orderReady = false;
            waitTimer = 0f;

            if (Station == null)
            {
                Debug.LogError("[NPCWaitForOrder]: Brain.CurrentOrderStation is null.");
                Brain.ChangeState("NPCLeaveRestaurant");
                return;
            }

            if (Station.WaitSpot == null)
            {
                Debug.LogError("[NPCWaitForOrder]: Station.WaitSpot is null.");
                Brain.ChangeState("NPCLeaveRestaurant");
                return;
            }

            if (Station.OrderSubmissionArea == null)
            {
                Debug.LogError("[NPCWaitForOrder]: Station.OrderSubmissionArea is null.");
                Brain.ChangeState("NPCLeaveRestaurant");
                return;
            }

            if (Brain.CurrentOrderID == 0)
            {
                Debug.LogError("[NPCWaitForOrder]: Brain.CurrentOrderID is invalid.");
                Brain.ChangeState("NPCLeaveRestaurant");
                return;
            }

            OrderManager.I.PlayerSubmittedOrder += OnPlayerSubmittedOrder;

            moveRoutine = StartCoroutine(MoveToWaitSpotCoroutine());
        }

        public override void OnExit(NPCBrain brain)
        {
            OrderManager.I.PlayerSubmittedOrder -= OnPlayerSubmittedOrder;

            if (moveRoutine != null)
            {
                StopCoroutine(moveRoutine);
                moveRoutine = null;
            }
        }

        public override void OnUpdate(NPCBrain brain)
        {
            if (orderReady)
                return;

            waitTimer += Time.deltaTime;

            if (waitTimer < maxWaitSeconds)
                return;

            string message = NPCDialoguePresets.RandomImpatientSaying();
            NPC.Dialogue.Say(message, 3f);

            OrderManager.I.CancelOrderIfExists(Brain.CurrentOrderID);

            Brain.CurrentOrderID = 0;

            Brain.ChangeState("NPCLeaveRestaurant");
        }

        public override void OnFixedUpdate(NPCBrain brain)
        {
        }

        private IEnumerator MoveToWaitSpotCoroutine()
        {
            yield return NPC.Movement.MoveAndAwait(Station.WaitSpot.position);
            moveRoutine = null;
        }

        private void OnPlayerSubmittedOrder(OrderSubmissionResult result)
        {
            if (orderReady)
                return;

            if (result.Order == null)
                return;

            if (result.Order.ID != Brain.CurrentOrderID)
                return;

            orderReady = true;
            Brain.ChangeState("NPCTakeOrder");
        }
    }
}