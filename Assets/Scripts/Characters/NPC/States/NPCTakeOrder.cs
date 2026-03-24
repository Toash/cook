using UnityEngine;

namespace Assets.Scripts.Characters.NPC
{
    public class NPCTakeOrder : NPCState
    {
        public override string Name => "NPCTakeOrder";

        private SingleOrderSubmissionArea submitArea;

        public override void OnEnter(NPCBrain brain)
        {
            if (Brain.CurrentOrderStation == null)
            {
                Debug.LogError("[NPCTakeOrder]: Brain.CurrentOrderStation is null.");
                Brain.ChangeState("NPCLeaveRestaurant");
                return;
            }

            submitArea = Brain.CurrentOrderStation.OrderSubmissionArea;

            if (submitArea == null)
            {
                Debug.LogError("[NPCTakeOrder]: OrderSubmissionArea is null.");
                Brain.ChangeState("NPCLeaveRestaurant");
                return;
            }

            if (submitArea.PickupSpot == null)
            {
                Debug.LogError("[NPCTakeOrder]: PickupSpot is null.");
                Brain.ChangeState("NPCLeaveRestaurant");
                return;
            }

            Brain.Agent.SetDestination(submitArea.PickupSpot.position);
        }

        public override void OnExit(NPCBrain brain)
        {
        }

        public override void OnFixedUpdate(NPCBrain brain)
        {
        }

        public override void OnUpdate(NPCBrain brain)
        {
            if (!AgentUtils.HasAgentReachedDestination(Brain.Agent))
                return;

            if (submitArea.TryTakeContainer(Brain.CurrentOrderID, out OrderContainer container))
            {
                Debug.Log("[NPC]: Picked up order");

                Brain.NPC.Hand.Hold(container.gameObject);

                OrderManager.I.OrderEvaluated(container.SubmissionResult);

                FinalOrderEvaluationResult eval = container.SubmissionResult.Evaluation;
                FinalOrderScoreCategory category = eval.ScoreCategory;
                string message = NPCDialoguePresets.RandomEvaluationSaying(category);

                NPC.Dialogue.Say(message);

                Brain.HasEatenToday = true;
                Brain.ChangeState("NPCLeaveRestaurant");
            }
        }
    }
}