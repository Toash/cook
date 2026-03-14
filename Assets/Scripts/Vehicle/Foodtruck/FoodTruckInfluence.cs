using Assets.Scripts.Vehicle;
using UnityEngine;

/// <summary>
/// Collider that infleunces customers
/// </summary>
[RequireComponent(typeof(Collider))]
public class FoodTruckInfluence : MonoBehaviour
{

    public FoodTruck FoodTruck;
    Collider col;

    void OnValidate()
    {
        if (col == null)
        {
            col = GetComponent<Collider>();
            col.isTrigger = true;
        }

    }

}