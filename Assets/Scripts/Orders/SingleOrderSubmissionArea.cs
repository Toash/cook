using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Auto submits when receciving an active order
/// </summary>
[RequireComponent(typeof(Collider))]
public class SingleOrderSubmissionArea : MonoBehaviour
{
    // the order that this submission area expects
    public UnityEvent OnOrderSuccessful;
    public UnityEvent OnOrderFailed;


    // private List<PreparedItem> preparedItems = new List<PreparedItem>();
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
            OnContainerAdded(container);
        }
    }


    void OnContainerAdded(OrderContainer container)
    {
        // check if can submit
        if (container.CanSubmit())
        {
            SubmitContainer(container);
        }
    }


    public void SubmitContainer(OrderContainer container)
    {
        int id = container.Receipt.OrderID;
        Order order = OrderManager.I.GetActiveOrderFromID(id);

        OrderSubmissionResult result = OrderManager.I.TrySubmit(order, container.PreparedItems);

        if (result.Status != OrderSubmissionStatus.Success)
        {
            Debug.Log("[OrderSubmissionArea]: Failed to submit PreparedItems for an Order.");
        }


        // destroy everything (for now)
        Destroy(container.Receipt.gameObject);
        foreach (PreparedItem item in container.PreparedItems)
        {
            Destroy(item.gameObject);
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
        message += "Submission Area";
        Handles.Label(transform.position, message);
    }

#endif

}