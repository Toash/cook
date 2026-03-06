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
    }
#endif
}