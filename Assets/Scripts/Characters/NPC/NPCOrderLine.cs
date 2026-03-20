
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

    private Coroutine moveRoutine;
    private bool hasProposedOrder;
    private bool awaitingAck;

    void MoveToCurrentLinePos()
    {
        if (FirstInLine()) return;
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToLinePosCoroutine());
    }

    IEnumerator MoveToLinePosCoroutine()
    {
        Vector3 currentLinePos = Brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.GetLinePositionForNPC(Brain);
        yield return NPC.Movement.MoveAndAwait(currentLinePos);

        moveRoutine = null;

        if (FirstInLine() && !hasProposedOrder && !awaitingAck)
        {
            ProposeOrder();
        }
    }

    void ProposeOrder()
    {
        if (hasProposedOrder || awaitingAck) return;

        Brain.Agent.SetDestination(Brain.CurrentFoodTruck.OrderSpot.position);

        hasProposedOrder = true;
        awaitingAck = true;

        OrderManager.I.ProposedOrderAcknowledged += OnProposedOrderAcknowledged;
        OrderProposition order = OrderManager.I.GenerateRandomOrderProposition(Brain.CurrentFoodTruck, Brain.NPC);
        OrderManager.I.ProposeOrder(order);
    }

    void OnProposedOrderAcknowledged(Order order)
    {
        if (order.Owner != Brain.NPC) return;

        hasProposedOrder = false;
        awaitingAck = false;

        Brain.CurrentOrderID = order.ID;

        OrderManager.I.ProposedOrderAcknowledged -= OnProposedOrderAcknowledged;
        Brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.RemoveNPCFromLine(Brain);
        Brain.ChangeState("NPCWaitForOrder");
    }
    bool FirstInLine()
    {
        return Brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.GetFirstNPCInLine() == Brain;

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

    // public override void OnExit(NPCBrain brain)
    // {
    //     Brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.RemoveNPCFromLineIfFirst(Brain);

    //     brain.LineChanged -= MoveToCurrentLinePos;
    //     OrderManager.I.ProposedOrderAcknowledged -= OnProposedOrderAcknowledged;
    //     Brain.NPC.Visuals.SetLookAtTarget(null);
    // }

    public override void OnExit(NPCBrain brain)
    {

        brain.LineChanged -= MoveToCurrentLinePos;
        OrderManager.I.ProposedOrderAcknowledged -= OnProposedOrderAcknowledged;
        Brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.RemoveNPCFromLine(brain);
        Brain.NPC.Visuals.SetLookAtTarget(null);

        hasProposedOrder = false;
        awaitingAck = false;
    }
    public override void OnFixedUpdate(NPCBrain brain)
    {
    }

    public override void OnUpdate(NPCBrain brain)
    {
        // move to some positio within the line.
    }


}