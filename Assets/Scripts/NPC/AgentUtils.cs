
using UnityEngine.AI;

public static class AgentUtils
{
    public static bool HasAgentReachedDestination(NavMeshAgent agent)
    {
        // 1. Check if a path is currently being computed (pathPending)
        if (!agent.pathPending)
        {
            // 2. Check if the remaining distance is less than or equal to the stopping distance
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // 3. Ensure the agent is not still computing a path and has no velocity
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }
}