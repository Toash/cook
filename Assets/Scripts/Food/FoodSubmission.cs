using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// place FootRoot here to give to npcs.
/// </summary>
[RequireComponent(typeof(Collider))]
public class FoodSubmission : MonoBehaviour
{
    // todo: explicit submit method?
    public Recipe matchingRecipe;
    List<FoodRoot> submittedFoodRoots = new List<FoodRoot>();

    private Collider col;



    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }


    void OnFoodRootAdded(FoodRoot root)
    {
        if (root.MatchesRecipe(matchingRecipe))
        {
            Debug.Log("Matches recipe!");
        }
    }
    void OnFoodRootRemoved(FoodRoot root)
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FoodIngredient>(out var ingredient))
        {
            // check for food root
            FoodRoot root = ingredient.GetFoodRoot();
            if (root != null)
            {
                if (submittedFoodRoots.Contains(root)) return;
                submittedFoodRoots.Add(root);
                OnFoodRootAdded(root);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<FoodIngredient>(out var ingredient))
        {
            // check for food root
            FoodRoot root = ingredient.GetFoodRoot();
            if (root != null)
            {
                if (submittedFoodRoots.Contains(root))
                {
                    submittedFoodRoots.Remove(root);
                    OnFoodRootRemoved(root);
                }
            }
        }
    }




#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.orange;

        string message = "";
        if (submittedFoodRoots.Count > 0)
        {
            message += "Food submitted\n";
            message += "Count: " + submittedFoodRoots.Count + "\n";
            Handles.Label(transform.position, message);
            Gizmos.DrawWireCube(transform.position, Vector3.one);
        }
        else
        {
            message += "Waiting for food...";
            Handles.Label(transform.position, message);

        }




    }

#endif

}