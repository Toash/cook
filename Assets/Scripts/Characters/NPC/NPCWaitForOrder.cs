public class NPCWaitForOrder : NPCState
{
    public override string StateName => "NPCWaitForOrder";

    void OnActiveOrderMade(Order order)
    {
        if (order.Owner == Brain.NPC)
        {
            Brain.ChangeState("NPCTakeOrder");
        }

    }
    public override void OnEnter(NPCBrain brain)
    {
        brain.Agent.SetDestination(Brain.CurrentFoodTruck.WaitingSpot.transform.position);
        OrderManager.I.ActiveOrderMade += OnActiveOrderMade;
    }

    public override void OnExit(NPCBrain brain)
    {
        OrderManager.I.ActiveOrderMade -= OnActiveOrderMade;
    }

    public override void OnFixedUpdate(NPCBrain brain)
    {
    }

    public override void OnUpdate(NPCBrain brain)
    {
    }


}