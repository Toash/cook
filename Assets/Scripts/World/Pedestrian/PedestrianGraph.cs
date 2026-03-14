using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Handles graph of sidewalk nodes. Contains methods for querying nodes.
/// </summary>
public class PedestrianGraph : MonoBehaviour
{
    public static PedestrianGraph I;

    [ShowInInspector, ReadOnly]
    private List<SidewalkNode> nodes = new();

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
    }

    public void RegisterNode(SidewalkNode node)
    {
        if (!nodes.Contains(node))
            nodes.Add(node);

        // nodes.Add(node);
    }

    public void UnregisterNode(SidewalkNode node)
    {
        nodes.Remove(node);
    }

    public SidewalkNode GetRandomNearbyNode(Vector3 pos, float radius, SidewalkNode exclude = null)
    {
        List<SidewalkNode> candidates = new();


        foreach (var node in nodes)
        {
            if (node == exclude)
                continue;

            if (Vector3.Distance(node.transform.position, pos) < radius)
            {
                candidates.Add(node);
            }
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    public SidewalkNode GetClosestNode(Vector3 pos)
    {
        SidewalkNode best = null;
        float bestDist = float.MaxValue;

        foreach (var node in nodes)
        {
            float dist = (node.transform.position - pos).sqrMagnitude;

            if (dist < bestDist)
            {
                bestDist = dist;
                best = node;
            }
        }

        return best;
    }

    public SidewalkNode GetRandomNode()
    {
        if (nodes.Count == 0)
            return null;

        return nodes[Random.Range(0, nodes.Count)];
    }

    public IReadOnlyList<SidewalkNode> Nodes => nodes;
}