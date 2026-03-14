using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FoodTruckInfluence : MonoBehaviour
{
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