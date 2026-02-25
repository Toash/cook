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
    public Order order;
    private OrderEvaluationResult orderEvalResult;
    public UnityEvent OnOrderSuccessful;
    public UnityEvent OnOrderFailed;
    List<PreparedItem> submittedItems = new List<PreparedItem>();

    private Collider col;




    void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }


    public void GenerateRandomOrder()
    {
        // Order order = Order.RandomOrder();
        Order order = OrderManager.I.GetRandomOrder();
        this.order = order;
        Debug.Log("Random order set");
    }
    public void SubmitPreparedItems()
    {
        // evaluate order
        List<PreparedItemData> submittedItemDatas = PreparedItemData.From(submittedItems);
        orderEvalResult = OrderEvaluator.Evaluate(order, submittedItemDatas);

        MoneyManager.I.AddMoney(order.Payout);
    }
    void OnMenuItemAdded(PreparedItem item)
    {
    }
    void OnMenuItemRemoved(PreparedItem root)
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Ingredient>(out var ingredient))
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
        if (other.TryGetComponent<Ingredient>(out var ingredient))
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
        if (order != null)
        {
            message += "Order menu items: \n";
            foreach (var menuItem in order.MenuItems)
            {
                message += menuItem.Name + "\n";
            }

            if (orderEvalResult == null)
            {
                // show submitted items
                if (submittedItems.Count > 0)
                {
                    Gizmos.color = Color.green;
                    message += "\nFood is in area:\n";
                    message += "Count: " + submittedItems.Count + "\n";
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