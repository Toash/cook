
using UnityEngine;

public class NPCTakeOrder : NPCState
{
    public override string StateName => "NPCTakeOrder";

    /// <summary>
    /// Where order will go when picked up
    /// </summary>
    //public Transform HandSocket;

    SingleOrderSubmissionArea submitArea;
    public override void OnEnter(NPCBrain brain)
    {
        submitArea = Brain.CurrentFoodTruck.OrderSubmissionArea;
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
        if (AgentUtils.HasAgentReachedDestination(Brain.Agent))
        {
            if (submitArea.TryTakeContainer(Brain.CurrentOrderID, out var container))
            {

                Debug.Log("[NPC]: Picked up order");
                //container.transform.SetParent(HandSocket, worldPositionStays: false);
                Brain.NPC.Hand.Hold(container.gameObject);

                Brain.ChangeState("NPCEatFood");

            }
        }
    }


}