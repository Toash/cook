using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Location NPCs will go to stand in line before ordering.
/// </summary>
public class OrderLine : MonoBehaviour
{
    /// <summary>
    /// The transform that NPCs in this line will look at.
    /// </summary>
    public Transform NPCLookAt;

    public float LineSpacing = 1.5f;
    public int MaxLineSize = 6;

    [ShowInInspector, ReadOnly]
    public List<NPCBrain> Line = new();

    /// <summary>
    /// Invoked whenever the line changes.
    /// Passes the first NPC in line.
    /// </summary>
    public event System.Action<NPCBrain> LineChanged;

    public NPCBrain GetFirstNPCInLine()
    {
        return Line.Count > 0 ? Line[0] : null;
    }

    public bool IsFirstInLine(NPCBrain npc)
    {
        return Line.Count > 0 && Line[0] == npc;
    }

    public bool CanJoinLine()
    {
        return Line.Count < MaxLineSize;
    }

    public Vector3 GetLinePositionForNPC(NPCBrain npc)
    {
        int index = Line.IndexOf(npc);

        if (index < 0)
        {
            Debug.LogWarning("[OrderLine]: NPC not found in line.");
            return transform.position;
        }

        return transform.position + transform.forward * LineSpacing * index;
    }

    public void AddNPCToLine(NPCBrain npc)
    {
        if (npc == null) return;
        if (Line.Contains(npc)) return;
        if (!CanJoinLine()) return;

        Line.Add(npc);
        LineChanged += npc.OnLineChanged;

        if (Line.Count > 0)
            LineChanged?.Invoke(Line[0]);
    }

    public void RemoveNPCFromLine(NPCBrain npc)
    {
        if (npc == null) return;
        if (!Line.Remove(npc)) return;

        LineChanged -= npc.OnLineChanged;

        if (Line.Count > 0)
        {
            LineChanged?.Invoke(Line[0]);
        }
    }

    public void ClearLine()
    {
        for (int i = Line.Count - 1; i >= 0; i--)
        {
            RemoveNPCFromLine(Line[i]);
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

        for (int i = 0; i < MaxLineSize; i++)
        {
            Gizmos.DrawSphere(transform.position + transform.forward * LineSpacing * i, .1f);
        }
    }
#endif
}