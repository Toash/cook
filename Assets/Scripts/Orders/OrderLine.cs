using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Location npcs will go to to propose an order.
/// </summary>
public class OrderLine : MonoBehaviour
{
    public bool IsOccupied { get; private set; }

    /// <summary>
    ///  NPCS in line
    ///  TODO: generate positions based on line position.
    /// </summary>
    [ShowInInspector, ReadOnly]
    public Queue<NPCBrain> Line = new();



    /// <summary>
    /// Invokves whenever the line changes, passes the first NPC in line first
    /// NPCs in this line should subscribe to this, to determine if they are first or not
    /// </summary>
    public event System.Action<NPCBrain> NewFirstInLine;



    public NPCBrain GetFirstNPCInLine()
    {
        return Line.Peek();
    }

    /// <summary>
    /// Adds an NPC to a line <br/>
    /// Subscribes the NPC to NewFirstInLine event.
    /// </summary>
    /// <param name="npc"></param>
    public void AddNPCToLine(NPCBrain npc)
    {
        Line.Enqueue(npc);
        NewFirstInLine += npc.OnFirstInLineChanged;
        // NewFirstInLine?.Invoke(Line.Peek());
    }


    /// <summary>
    /// Removes an NPC from a line <br/>
    /// Unsubscribes from the NewFirstInLine event. <br/>
    /// Calling this method first the NewFirstInLine event for the next NPC.
    /// </summary>
    /// <param name="npc"></param>
    public bool RemoveNPCFromLineIfFirst(NPCBrain npc)
    {
        var firstNpcInLine = Line.Peek();
        if (npc != firstNpcInLine) return false;
        NewFirstInLine -= firstNpcInLine.OnFirstInLineChanged;
        Line.Dequeue();
        NewFirstInLine?.Invoke(Line.Peek());
        return true;
    }



#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, .3f);

        Handles.Label(transform.position, "OrderLine");
    }
#endif




}