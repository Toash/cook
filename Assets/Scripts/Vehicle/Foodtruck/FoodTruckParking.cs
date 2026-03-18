using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Represents a place to part the foodtruck. contains references to order stuff
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class FoodTruckParking : MonoBehaviour
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
    void Awake()
    {
        if (OrderLine == null) Debug.LogError("[Parking]: OrderLine is null!");
        if (WaitingSpot == null) Debug.LogError("[Parking]: WaitingSpot is null!");

    }
    void Start()
    {
        FoodTruckParkingSpotManager.Instance.Register(this);
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