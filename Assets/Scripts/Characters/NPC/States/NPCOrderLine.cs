using System.Collections;
using UnityEngine;

/// <summary>
/// State for when NPC is standing in the order line.
/// If first in line, they propose an order.
/// </summary>
public class NPCOrderLine : NPCState
{
    public override string Name => "NPCOrderLine";

    private Coroutine moveRoutine;
    private bool hasProposedOrder;

    private OrderStation Station => Brain.CurrentOrderStation;
    private OrderLine Line => Station != null ? Station.OrderLine : null;

    public override void OnEnter(NPCBrain brain)
    {
        if (Station == null)
        {
            Debug.LogError("[NPCOrderLine]: Brain.CurrentOrderStation is null.");
            Brain.ChangeState("NPCLeaveRestaurant");
            return;
        }

        if (Line == null)
        {
            Debug.LogError("[NPCOrderLine]: Station.OrderLine is null.");
            Brain.ChangeState("NPCLeaveRestaurant");
            return;
        }

        Line.AddNPCToLine(brain);

        if (Line.NPCLookAt != null)
        {
            NPC.Visuals.SetLookAtTarget(Line.NPCLookAt);
        }

        brain.LineChanged += MoveToCurrentLinePos;
        MoveToCurrentLinePos();
    }

    public override void OnExit(NPCBrain brain)
    {
        brain.LineChanged -= MoveToCurrentLinePos;
        OrderManager.I.ProposedOrderAcknowledged -= OnProposedOrderAcknowledged;

        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }

        if (Line != null)
        {
            Line.RemoveNPCFromLine(brain);
        }

        NPC.Visuals.SetLookAtTarget(null);
        hasProposedOrder = false;
    }

    public override void OnUpdate(NPCBrain brain)
    {
    }

    public override void OnFixedUpdate(NPCBrain brain)
    {
    }

    private void MoveToCurrentLinePos()
    {
        if (hasProposedOrder)
            return;

        if (Line == null)
            return;

        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }

        moveRoutine = StartCoroutine(MoveToLinePosCoroutine());
    }

    private IEnumerator MoveToLinePosCoroutine()
    {
        Vector3 currentLinePos = Line.GetLinePositionForNPC(Brain);
        yield return NPC.Movement.MoveAndAwait(currentLinePos);

        moveRoutine = null;

        if (FirstInLine() && !hasProposedOrder)
        {
            ProposeOrder();
        }
    }

    private void ProposeOrder()
    {
        if (hasProposedOrder)
            return;

        if (Station == null)
        {
            Debug.LogError("[NPCOrderLine]: Station is null.");
            return;
        }

        if (Station.OrderSpot == null)
        {
            Debug.LogError("[NPCOrderLine]: Station.OrderSpot is null.");
            return;
        }

        hasProposedOrder = true;

        Brain.Agent.SetDestination(Station.OrderSpot.position);

        OrderManager.I.ProposedOrderAcknowledged += OnProposedOrderAcknowledged;

        OrderProposition order = OrderManager.I.GenerateRandomOrderProposition(Station, Brain.NPC);
        OrderManager.I.ProposeOrder(order);
    }

    private void OnProposedOrderAcknowledged(Order order)
    {
        if (order.Owner != Brain.NPC)
            return;

        OrderManager.I.ProposedOrderAcknowledged -= OnProposedOrderAcknowledged;

        Brain.CurrentOrderID = order.ID;

        if (Line != null)
        {
            Line.RemoveNPCFromLine(Brain);
        }

        hasProposedOrder = false;
        Brain.ChangeState("NPCWaitForOrder");
    }

    private bool FirstInLine()
    {
        if (Line == null)
            return false;

        return Line.GetFirstNPCInLine() == Brain;
    }
}