
using System.Collections;
using UnityEngine;

/// <summary>
/// State for when npc is in an orderline
/// 
/// First in line proposes an order
/// </summary>
public class NPCOrderLine : NPCState
{

    public override string Name => "NPCOrderLine";
    bool FirstInLine()
    {
        return Brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.GetFirstNPCInLine() == Brain;

    }

    void OnProposedOrderAcknowledged(Order order)
    {
        if (order.Owner != Brain.NPC) return;
        Brain.CurrentOrderID = order.ID;

        Brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.RemoveNPCFromLineIfFirst(Brain);
        Brain.ChangeState("NPCWaitForOrder");


    }
    void ProposeOrder()
    {
        Debug.Log("[NPC]: First in line");
        // Brain.ChangeState("NPCOrdering");

        OrderManager.I.ProposedOrderAcknowledged += OnProposedOrderAcknowledged;
        OrderProposition order = OrderManager.I.GenerateRandomOrderProposition(Brain.NPC);
        OrderManager.I.ProposeOrder(order);
    }
    void MoveToCurrentLinePos()
    {
        StartCoroutine(MoveToLinePosCoroutine());

    }

    IEnumerator MoveToLinePosCoroutine()
    {
        // StopAllCoroutines();
        Vector3 currentLinePos = Brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.GetLinePositionForNPC(Brain);
        yield return NPC.Movement.MoveAndAwait(currentLinePos);

        if (FirstInLine())
        {
            ProposeOrder();
        }

    }
    public override void OnEnter(NPCBrain brain)
    {

        if (Brain.CurrentFoodTruck == null)
        {
            Debug.LogError("[NPC]: Current food truck is null!");
        }
        Brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.AddNPCToLine(brain);
        NPC.Visuals.SetLookAtTarget(brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.NPCLookAt);

        MoveToCurrentLinePos();
        brain.LineChanged += MoveToCurrentLinePos;

    }

    public override void OnExit(NPCBrain brain)
    {
        Brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.RemoveNPCFromLineIfFirst(Brain);

        brain.LineChanged -= MoveToCurrentLinePos;
        OrderManager.I.ProposedOrderAcknowledged -= OnProposedOrderAcknowledged;
        Brain.NPC.Visuals.SetLookAtTarget(null);
    }

    public override void OnFixedUpdate(NPCBrain brain)
    {
    }

    public override void OnUpdate(NPCBrain brain)
    {
        // move to some positio within the line.
    }


}