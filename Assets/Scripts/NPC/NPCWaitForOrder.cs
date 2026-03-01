
using UnityEngine;
using UnityEngine.AI;

public class NPCWaitForOrder : NPCState
{
    public override string StateName => "NPCWaitForOrder";

    public override void OnEnter(NPCBrain brain)
    {
        brain.Agent.SetDestination(OrderManager.I.WaitingSpot.transform.position);
    }

    public override void OnExit(NPCBrain brain)
    {
    }

    public override void OnFixedUpdate(NPCBrain brain)
    {
    }

    public override void OnUpdate(NPCBrain brain)
    {
    }


}