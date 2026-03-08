using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMovement : Moveable
{

    public NavMeshAgent Agent;

    void OnValidate()
    {
        if (Agent == null)
            Agent = GetComponent<NavMeshAgent>();
    }
    List<SidewalkNode> currentPath;
    int pathIndex;
    void Update()
    {
        if (currentPath == null || pathIndex >= currentPath.Count)
            return;

        // if (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance)
        if (AgentUtils.HasAgentReachedDestination(Agent))
        {
            pathIndex++;
            MoveToNextNode();
        }
    }
    public void SetNodePath(List<SidewalkNode> path)
    {
        currentPath = path;
        pathIndex = 0;

        MoveToNextNode();
    }

    void MoveToNextNode()
    {
        if (pathIndex >= currentPath.Count)
            return;

        Agent.SetDestination(currentPath[pathIndex].transform.position);
    }

    public override float GetMoveSpeed()
    {
        return Agent.velocity.magnitude;
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (Agent != null)
        {
            Handles.Label(transform.position + Vector3.up * 2, $"Speed: {Agent.velocity.magnitude:F2}");
        }

        if (currentPath == null) return;
        foreach (var node in currentPath)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(node.transform.position, .5f);
        }
    }
#endif
}