
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Goes to an OrderLocation and waits in line.
/// </summary>
public class NPCGoToRegister : NPCState
{
    // OrderLocation orderLocation;

    public override string StateName => "NPCGoToRegister";

    public override void OnEnter(NPCBrain brain)
    {
        brain.SetCurrentOrderLocation(OrderManager.I.OrderLocation);
        // go to order location
        brain.Agent.SetDestination(brain.GetCurrentOrderLocation().transform.position);
    }

    public override void OnExit(NPCBrain brain)
    {
    }

    public override void OnFixedUpdate(NPCBrain brain)
    {
    }

    public override void OnUpdate(NPCBrain brain)
    {
        // reached the register.
        if (AgentUtils.HasAgentReachedDestination(brain.Agent))
        {
            brain.ChangeState("NPCWaitInLine");

        }
    }
}