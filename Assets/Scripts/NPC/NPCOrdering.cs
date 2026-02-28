
using UnityEngine;
using UnityEngine.AI;

public class NPCOrdering : NPCState
{
    public override string StateName => "NPCOrdering";

    public override void OnEnter(NPCBrain brain)
    {
        var order = OrderManager.I.GenerateRandomOrder();
        OrderManager.I.ProposeOrder(order);
    }

    public override void OnExit(NPCBrain brain)
    {
        brain.ExitLine();
    }

    public override void OnFixedUpdate(NPCBrain brain)
    {
    }

    public override void OnUpdate(NPCBrain brain)
    {
    }


}