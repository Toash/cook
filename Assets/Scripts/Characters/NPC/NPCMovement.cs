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
    public System.Collections.IEnumerator MoveAndAwait(Vector3 pos)
    {
        Agent.SetDestination(pos);

        // wait until the path is ready
        yield return new WaitUntil(() => !Agent.pathPending);

        // wait until the agent reaches the destination
        while (!AgentUtils.HasAgentReachedDestination(Agent))
        {
            yield return null;
        }

    }



#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (Agent != null)
        {
            Handles.Label(transform.position + Vector3.up * 2, $"Speed: {Agent.velocity.magnitude:F2}");
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(Agent.destination, .2f);
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