using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Represents a place to part the truck. 
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class TruckParking : MonoBehaviour
{
    [Tooltip("Where npcs will wait in line.")]
    public OrderLine OrderLine;

    [Tooltip("Where npcs will wait after ordering.")]
    public OrderWaitingSpot WaitingSpot;

    SphereCollider col;

    void OnValidate()
    {
        if (col == null)
        {
            col = GetComponent<SphereCollider>();
            col.isTrigger = true;
        }

    }



#if UNITY_EDITOR
    GUIStyle style = new GUIStyle();
    void OnDrawGizmos()
    {
        style.normal.textColor = Color.blue;
        style.fontSize = 18;
        Handles.Label(transform.position + Vector3.up, "Parking", style);
    }

#endif


}