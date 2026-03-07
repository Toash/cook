using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Location npcs will go to to propose an order.
/// </summary>
public class OrderLine : MonoBehaviour
{
    public bool IsOccupied { get; private set; }

    /// <summary>
    /// The transform that npcs in this line will look at. <br/>
    /// </summary>
    public Transform NPCLookAt;
    public float LineSpacing = 1f;
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
    public Vector3 GetLinePositionForNPC(NPCBrain npc)
    {
        int index = 0;
        foreach (var npcInLine in Line)
        {
            if (npcInLine == npc)
            {
                break;
            }
            index++;
        }
        return transform.position + transform.forward * LineSpacing * index;
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
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);

        Handles.Label(transform.position, "OrderLine");
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var npc in Line)
        {
            var pos = GetLinePositionForNPC(npc);

            Gizmos.DrawSphere(pos, .2f);
        }



    }
#endif




}