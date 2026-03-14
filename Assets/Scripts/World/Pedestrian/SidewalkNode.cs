using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Node to mark sidewalks that the NPCs will prefer to walk on.
/// </summary>
public class SidewalkNode : MonoBehaviour
{
    public PedestrianGraph Graph;
    public List<SidewalkNode> Neighbors = new();

    void OnValidate()
    {
        if (Graph == null)
        {
            Graph = FindAnyObjectByType<PedestrianGraph>();
        }

    }

    void OnEnable()
    {
        Graph.RegisterNode(this);
        Debug.Log("[SidewalkNode]: Registering");
    }

    void OnDisable()
    {
        Graph.UnregisterNode(this);
    }

#if UNITY_EDITOR
    GUIStyle style = new GUIStyle();
    private void OnDrawGizmos()
    {
        style.normal.textColor = Color.red;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, .5f);
        if (Neighbors.Count == 0)
        {
            Handles.Label(transform.position + Vector3.up * 2f, "neighbors not set!", style);

        }
        style.normal.textColor = Color.green;
        Handles.Label(transform.position + Vector3.up, gameObject.name, style);

        if (Neighbors == null) return;

        foreach (var neighbor in Neighbors)
        {
            Gizmos.DrawLine(transform.position, neighbor.transform.position);
            // draw arrow from neighbor to this node
            float size = 2;
            // Vector3 dir = (transform.position - neighbor.transform.position).normalized;
            Vector3 dir = (neighbor.transform.position - transform.position).normalized;
            Quaternion rot = Quaternion.LookRotation(dir);
            // Handles.color = Handles.yAxisColor;
            Handles.ArrowHandleCap(0, (transform.position + dir * size * 1.25f) + Vector3.up, rot, size, EventType.Repaint);

        }

    }
#endif

}