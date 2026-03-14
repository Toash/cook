
using UnityEngine;

public class NPCWaitInLine : NPCState
{

    public override string Name => "NPCWaitInLine";

    void OnNewFirstInLine(NPCBrain brain)
    {
        Debug.Log("[NPC]: First in line");
        brain.ChangeState("NPCOrdering");
    }
    void OnLineChanged()
    {
        Brain.Agent.SetDestination(Brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.GetLinePositionForNPC(Brain));
    }
    public override void OnEnter(NPCBrain brain)
    {

        if (Brain.CurrentFoodTruck == null)
        {
            Debug.LogError("[NPC]: Current food truck is null!");
        }
        Brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.AddNPCToLine(brain);
        NPC.Visuals.SetLookAtTarget(brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.NPCLookAt);


        // go to line position
        brain.Agent.SetDestination(brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.GetLinePositionForNPC(brain));
        brain.LineChanged += OnLineChanged;

        // already first in line
        if (brain.CurrentFoodTruck.CurrentParkingSpot.OrderLine.GetFirstNPCInLine() == brain)
        {
            OnNewFirstInLine(brain);
            return;
        }
        // not already first in line, subscribe to event that tells us when this npc is first.
        brain.BecameFirstInLine += OnNewFirstInLine;
    }

    public override void OnExit(NPCBrain brain)
    {
        brain.BecameFirstInLine -= OnNewFirstInLine;
        brain.LineChanged -= OnLineChanged;
    }

    public override void OnFixedUpdate(NPCBrain brain)
    {
    }

    public override void OnUpdate(NPCBrain brain)
    {
        // move to some positio within the line.
    }


}