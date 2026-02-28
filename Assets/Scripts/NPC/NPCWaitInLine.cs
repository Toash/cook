
using UnityEngine;
using UnityEngine.AI;

public class NPCWaitInLine : NPCState
{

    public override string StateName => "NPCWaitInLine";

    public override void OnEnter(NPCBrain brain)
    {
        brain.BecameFirstInLine += OnNewFirstInLine;
    }

    public override void OnExit(NPCBrain brain)
    {
        brain.BecameFirstInLine -= OnNewFirstInLine;
    }

    public override void OnFixedUpdate(NPCBrain brain)
    {
    }

    public override void OnUpdate(NPCBrain brain)
    {
        // move to some positio within the line.
    }


    void OnNewFirstInLine(NPCBrain brain)
    {
        // go to order
        brain.ChangeState("NPCOrdering");
    }
}