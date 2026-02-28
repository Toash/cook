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


    // private List<PreparedItem> preparedItems = new List<PreparedItem>();
    private OrderContainer container;
    private Collider col;
    private OrderEvaluationResult orderEvalResult;


    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<OrderContainer>(out var container))
        {
            this.container = container;
            OnContainerAdded(container);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<OrderContainer>(out var container))
        {
            this.container = container;
            OnContainerRemoved(container);
        }
    }


    void OnContainerAdded(OrderContainer container)
    {

    }
    void OnContainerRemoved(OrderContainer container)
    {

    }


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
        OrderSubmissionResult result = OrderManager.I.TrySubmit(Order, container.PreparedItems);
        if (result.Status != OrderSubmissionStatus.Success)
        {
            Debug.Log("[OrderSubmissionArea]: Failed to submit PreparedItems for an Order.");
        }

        Destroy(container.gameObject);
    }

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
                if (container != null)
                {
                    Gizmos.color = Color.green;
                    message += "\nFood is in area:\n";
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