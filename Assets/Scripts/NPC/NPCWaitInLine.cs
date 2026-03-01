
using UnityEngine;
using UnityEngine.AI;

public class NPCWaitInLine : NPCState
{

    public override string StateName => "NPCWaitInLine";

    void OnNewFirstInLine(NPCBrain brain)
    {
        Debug.Log("[NPC]: First in line");
        brain.ChangeState("NPCOrdering");
    }
    public override void OnEnter(NPCBrain brain)
    {
        brain.CurrentOrderLine.AddNPCToLine(brain);
        // already first in line
        if (brain.CurrentOrderLine.GetFirstNPCInLine() == brain)
        {
            OnNewFirstInLine(brain);
            return;
        }

        // not already first in line, subscribe to event that tells us when .
        brain.BecameFirstInLine += OnNewFirstInLine;
    }

    public override void OnExit(NPCBrain brain)
    {
    }

    public override void OnFixedUpdate(NPCBrain brain)
    {
    }

    public override void OnUpdate(NPCBrain brain)
    {
        // move to some positio within the line.
    }


}