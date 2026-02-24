using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Collider))]
public class OrderSubmissionArea : MonoBehaviour
{
    public Order order;
    public UnityEvent OnOrderSuccessful;
    public UnityEvent OnOrderFailed;
    List<PreparedItem> submittedItems = new List<PreparedItem>();

    private Collider col;




    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }


    public void Submit()
    {
        // evaluate order

        List<PreparedItemSnapshot> submittedItemSnapshots = PreparedItemSnapshot.From(submittedItems);

        OrderEvaluator.Evaluate(order, submittedItemSnapshots);

        // fire off appropriate event.
    }
    void OnMenuItemAdded(PreparedItem item)
    {
        // 

        // if (item.MatchesMenuItem())
        // {
        //     Debug.Log("Matches recipe!");
        // }
    }
    void OnMenuItemRemoved(PreparedItem root)
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<FoodIngredient>(out var ingredient))
        {
            // check for food root
            PreparedItem root = ingredient.PreparedItem;
            if (root != null)
            {
                if (submittedItems.Contains(root)) return;
                submittedItems.Add(root);
                OnMenuItemAdded(root);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<FoodIngredient>(out var ingredient))
        {
            // check for food root
            PreparedItem root = ingredient.PreparedItem;
            if (root != null)
            {
                if (submittedItems.Contains(root))
                {
                    submittedItems.Remove(root);
                    OnMenuItemRemoved(root);
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
        if (submittedItems.Count > 0)
        {
            message += "Food submitted\n";
            message += "Count: " + submittedItems.Count + "\n";
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