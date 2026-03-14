public class NPCOrdering : NPCState
{
    public override string Name => "NPCOrdering";


    void OnProposedOrderAcknowledged(Order order)
    {
        if (order.Owner != Brain.NPC) return;
        Brain.CurrentOrderID = order.ID;

        Brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.RemoveNPCFromLineIfFirst(Brain);
        Brain.ChangeState("NPCWaitForOrder");


    }
    public override void OnEnter(NPCBrain brain)
    {
        // NPC is still pathfinding wait until arrived at destination.
        OrderManager.I.ProposedOrderAcknowledged += OnProposedOrderAcknowledged;
        var prop = OrderManager.I.GenerateRandomOrderProposition(brain.NPC);
        OrderManager.I.ProposeOrder(prop);
    }

    public override void OnExit(NPCBrain brain)
    {
        OrderManager.I.ProposedOrderAcknowledged -= OnProposedOrderAcknowledged;
        Brain.NPC.Visuals.SetLookAtTarget(null);
    }

    public override void OnFixedUpdate(NPCBrain brain)
    {
    }

    public override void OnUpdate(NPCBrain brain)
    {
    }


}