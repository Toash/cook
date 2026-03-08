using System.Collections.Generic;
using UnityEngine;

public static class PedestrianPathfinder
{
    /// <summary>
    /// BFS to find path of sidewalk nodes.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    /// <returns></returns>
    public static List<SidewalkNode> FindPath(SidewalkNode start, SidewalkNode goal)
    {
        if (start == null || goal == null)
            return null;

        var frontier = new Queue<SidewalkNode>();
        var cameFrom = new Dictionary<SidewalkNode, SidewalkNode>();

        frontier.Enqueue(start);
        cameFrom[start] = null;

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current == goal)
                break;

            foreach (var next in current.Neighbors)
            {
                if (next == null || cameFrom.ContainsKey(next))
                    continue;

                frontier.Enqueue(next);
                cameFrom[next] = current;
            }
        }

        if (!cameFrom.ContainsKey(goal))
            return null;

        var path = new List<SidewalkNode>();
        var cur = goal;

        while (cur != null)
        {
            path.Add(cur);
            cur = cameFrom[cur];
        }

        path.Reverse();
        return path;
    }
}