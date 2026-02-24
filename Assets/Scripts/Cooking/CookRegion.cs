using UnityEngine;

/// <summary>
/// A region that can cook a cookable based on some type
/// </summary>
[RequireComponent(typeof(Collider))]
public class CookRegion : MonoBehaviour
{

    [Tooltip("The type that this region cooks.")]
    public CookType CookType;
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