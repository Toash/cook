using Mono.Cecil.Cil;
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
    public float LineSpacing = 1.5f;
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
    public event System.Action<NPCBrain> LineChanged;



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

    public void AddNPCToLine(NPCBrain npc)
    {
        Line.Enqueue(npc);
        LineChanged += npc.OnLineChanged;
        // NewFirstInLine?.Invoke(Line.Peek());
        LineChanged?.Invoke(Line.Peek());
    }
    /// <summary>
    /// Removes an npc from the line. Does not necessarily remove the first.
    /// </summary>
    /// <param name="npc"></param>
    public void RemoveNPCFromLineIfFirst(NPCBrain npc)
    {
        var firstNPC = Line.Peek();
        if (firstNPC != npc) return;

        Line.Dequeue();
        LineChanged -= firstNPC.OnLineChanged;

        LineChanged?.Invoke(firstNPC);
    }

    public void ClearLine()
    {
        NPCBrain npc;
        while ((npc = Line.Peek()) != null)
        {
            RemoveNPCFromLineIfFirst(npc);

        }

    }



#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, .3f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);

        Handles.Label(transform.position, "OrderLine");
        foreach (var npc in Line)
        {
            var pos = GetLinePositionForNPC(npc);

            Gizmos.DrawSphere(pos, .2f);
        }

        for (int i = 0; i < 8; i++)
        {
            Gizmos.DrawSphere(transform.position + transform.forward * LineSpacing * i, .1f);
        }



    }
#endif




}