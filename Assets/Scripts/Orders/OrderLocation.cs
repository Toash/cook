using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Location npcs will go to to propose an order.
/// </summary>
public class OrderLocation : MonoBehaviour
{
    public bool IsOccupied { get; private set; }

    /// <summary>
    ///  NPCS in line
    /// </summary>
    public Queue<NPCBrain> Line = new();



    /// <summary>
    /// Invokves whenever the line changes
    /// NPCs in this line should subscribe to this, to determine if they are first or not
    /// </summary>
    public event System.Action<NPCBrain> NowFirstInLine;



    // add to line
    // check if first in line
    public void AddToLine(NPCBrain npc)
    {
        Line.Enqueue(npc);
        NowFirstInLine?.Invoke(npc);
    }


    // check who is the first line
    public void RemoveFromLine()
    {
        Line.Dequeue();
        NowFirstInLine?.Invoke(Line.Peek());
    }



#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, .3f);

        Handles.Label(transform.position, "OrderLocation");
    }
#endif




}