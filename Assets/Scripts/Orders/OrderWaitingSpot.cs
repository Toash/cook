using UnityEditor;
using UnityEngine;

public class OrderWaitingSpot : MonoBehaviour
{

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.Label(transform.position, "Order Waiting Spot");

    }
#endif
}