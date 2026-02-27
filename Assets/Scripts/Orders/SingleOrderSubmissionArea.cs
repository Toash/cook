using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Submission area for a single order
/// </summary>
[RequireComponent(typeof(Collider))]
public class SingleOrderSubmissionArea : MonoBehaviour
{
    // the order that this submission area expects
    public Order Order;
    private bool hasOrder = false;
    public UnityEvent OnOrderSuccessful;
    public UnityEvent OnOrderFailed;

    private List<PreparedItem> preparedItems = new List<PreparedItem>();
    private Collider col;
    private OrderEvaluationResult orderEvalResult;




    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }
    // void FixedUpdate()
    // {
    //     CheckForNullPreparedItems();
    // }


    public void GenerateRandomOrder()
    {
        // Order order = Order.RandomOrder();
        Order order = OrderManager.I.AddRandomOrder();
        this.Order = order;
        hasOrder = true;
        Debug.Log("Random order set");
    }
    public void SubmitPreparedItems()
    {
        OrderSubmissionResult result = OrderManager.I.TrySubmit(Order, preparedItems);
        if (result.Status != OrderSubmissionStatus.Success)
        {
            Debug.Log("[OrderSubmissionArea]: Failed to submit PreparedItems for an Order.");
        }


        foreach (PreparedItem item in preparedItems)
        {
            Destroy(item.gameObject);
        }
    }
    void OnMenuItemAdded(PreparedItem item)
    {
    }
    void OnMenuItemRemoved(PreparedItem root)
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (hasOrder == false) return;
        if (other.TryGetComponent<Ingredient>(out var ingredient))
        {
            PreparedItem preparedItem = ingredient.PreparedItem;
            if (preparedItem != null)
            {
                if (preparedItems.Contains(preparedItem)) return;
                preparedItems.Add(preparedItem);
                preparedItem.Destroyed += OnPreparedItemDestroyed;

                OnMenuItemAdded(preparedItem);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (hasOrder == false) return;
        if (other.TryGetComponent<Ingredient>(out var ingredient))
        {
            // check for food root
            PreparedItem preparedItem = ingredient.PreparedItem;
            if (preparedItem != null)
            {
                if (preparedItems.Contains(preparedItem))
                {
                    preparedItems.Remove(preparedItem);
                    preparedItem.Destroyed -= OnPreparedItemDestroyed;
                    OnMenuItemRemoved(preparedItem);
                }
            }
        }
    }

    void OnPreparedItemDestroyed(PreparedItem item)
    {
        preparedItems.Remove(item);
    }

    // void CheckForNullPreparedItems()
    // {
    //     foreach (var item in preparedItems)
    //     {

    //         if (item == null)
    //         {
    //             preparedItems.Remove(item);
    //         }
    //     }
    // }



#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.orange;

        string message = "";
        if (Order != null)
        {
            message += "Order menu items: \n";
            foreach (var menuItem in Order.MenuItems)
            {
                message += menuItem.Name + "\n";
            }

            if (orderEvalResult == null)
            {
                // show submitted items
                if (preparedItems.Count > 0)
                {
                    Gizmos.color = Color.green;
                    message += "\nFood is in area:\n";
                    message += "Count: " + preparedItems.Count + "\n";
                    Handles.Label(transform.position, message);
                    Gizmos.DrawWireCube(transform.position, Vector3.one);
                }
                else
                {
                    Gizmos.color = Color.red;
                    message += "Waiting for food...";
                    Handles.Label(transform.position, message);
                    Gizmos.DrawWireCube(transform.position, Vector3.one);

                }


            }
            else
            {
                // show result
                message += "Result:\n";
                message += orderEvalResult + "\n";

                Handles.Label(transform.position, message);
            }

        }
        else
        {
            message += "Waiting for order...";
            Handles.Label(transform.position, message);
        }
    }

#endif

}