public class NPCWaitForOrder : NPCState
{
    public override string Name => "NPCWaitForOrder";

    void OnActiveOrderMade(OrderSubmissionResult result)
    {
        if (result.Order.Owner == Brain.NPC)
        {
            Brain.ChangeState("NPCTakeOrder");
        }

    }
    public override void OnEnter(NPCBrain brain)
    {
        brain.Agent.SetDestination(Brain.CurrentFoodTruck.CurrentParkingSpot.WaitingSpot.transform.position);
        OrderManager.I.PlayerSubmittedOrder += OnActiveOrderMade;
    }

    public override void OnExit(NPCBrain brain)
    {
        OrderManager.I.PlayerSubmittedOrder -= OnActiveOrderMade;
    }

    public override void OnFixedUpdate(NPCBrain brain)
    {
    }

    public override void OnUpdate(NPCBrain brain)
    {
    }


}