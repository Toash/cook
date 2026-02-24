using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Grill : MonoBehaviour
{
    // the collider that cooks
    Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();
    }


    void OnTriggerStay(Collider other)
    {

        if (other.TryGetComponent<Cookable>(out var cookable))
        {
            cookable.Cook(Time.fixedDeltaTime);
        }
    }
}