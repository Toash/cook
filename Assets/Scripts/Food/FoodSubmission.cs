using UnityEditor;
using UnityEngine;

/// <summary>
/// place FootRoot here to give to npcs.
/// </summary>
public class FoodSubmission : MonoBehaviour
{
    FoodRoot submittedFoodRoot;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FoodRoot>(out var foodroot))
        {
            Debug.Log("Got food root.");
        }

    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (submittedFoodRoot != null)
        {
            Handles.Label(transform.position, "Food submitted");
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }

    }

#endif

}