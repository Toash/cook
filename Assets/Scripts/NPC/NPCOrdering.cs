
using UnityEngine;
using UnityEngine.AI;

public class NPCOrdering : NPCState
{
    public override string StateName => "NPCOrdering";


    void OnProposedOrderAcknowledged(Order order)
    {
        if (order.Owner != Brain.NPC) return;

        Brain.ChangeState("NPCWaitForOrder");


    }
    public override void OnEnter(NPCBrain brain)
    {
        OrderManager.I.ProposedOrderAcknowledged += OnProposedOrderAcknowledged;
        var order = OrderManager.I.GenerateRandomOrderProposition(brain.NPC);
        OrderManager.I.ProposeOrder(order);
    }

    public override void OnExit(NPCBrain brain)
    {
        OrderManager.I.ProposedOrderAcknowledged -= OnProposedOrderAcknowledged;
        // if (brain.CurrentOrderLine.RemoveNPCFromLineIfFirst(brain))
        // {
        //     Destroy(brain.gameObject);
        // }
        // else
        // {
        //     Debug.LogError("[NPC]: Trying to leave line from Ordering but is not first in line.");
        // }
    }

    public override void OnFixedUpdate(NPCBrain brain)
    {
    }

    public override void OnUpdate(NPCBrain brain)
    {
    }


}