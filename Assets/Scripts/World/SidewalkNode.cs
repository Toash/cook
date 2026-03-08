using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Node to mark sidewalks that the NPCs will prefer to walk on.
/// </summary>
public class SidewalkNode : MonoBehaviour
{
    public List<SidewalkNode> Neighbors = new();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, .2f);

        if (Neighbors == null) return;

        foreach (var neighbor in Neighbors)
        {
            Gizmos.DrawLine(transform.position, neighbor.transform.position);
        }

    }

}