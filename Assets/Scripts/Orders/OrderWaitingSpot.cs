using UnityEditor;
using UnityEngine;

public class OrderWaitingSpot : MonoBehaviour
{

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Handles.Label(transform.position, "Order Waiting Spot");

    }
#endif
}