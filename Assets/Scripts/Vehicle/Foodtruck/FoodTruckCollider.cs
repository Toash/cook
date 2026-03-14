using System;
using UnityEngine;

/// <summary>
/// Collider for the entire food truck for detection.
/// </summary>
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class FoodTruckCollider : MonoBehaviour
{

    public event Action<TruckParking> EnteredParking;
    public event Action<TruckParking> ExitedParking;

    BoxCollider col;
    void OnValidate()
    {
        if (col == null)
        {
            col = GetComponent<BoxCollider>();
            col.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<TruckParking>(out var parking))
        {
            Debug.Log("[FoodTruck]: Enter parking spot");
            EnteredParking?.Invoke(parking);

        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<TruckParking>(out var parking))
        {
            Debug.Log("[FoodTruck]: Exit parking spot");
            ExitedParking?.Invoke(parking);


        }
    }
}