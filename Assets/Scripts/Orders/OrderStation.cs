using System.Collections.Generic;
using Assets.Scripts.Ingredient.MenuItem;
using UnityEngine;

public class OrderStation : MonoBehaviour
{
    [Tooltip("The line that customers wait in to place orders at this station.")]
    public OrderLine OrderLine;
    [Tooltip("Where customers go before enterring the order line.")]
    public Transform OrderLineEntryPoint;

    [Tooltip("Where customers stand to place their order.")]
    public Transform OrderSpot;

    [Tooltip("Where customers wait while their order is being prepared.")]
    public Transform WaitSpot;

    public SingleOrderSubmissionArea OrderSubmissionArea;

    [Tooltip("Where customers go before exiting the restaurant.")]
    public Transform ExitSpot;

    public List<OrderMenuItem> AvailableMenuItems;

    private void Awake()
    {
        if (OrderLine == null)
            Debug.LogError("[OrderStation]: OrderLine is null.", this);

        if (OrderSpot == null)
            Debug.LogError("[OrderStation]: OrderSpot is null.", this);

        if (WaitSpot == null)
            Debug.LogError("[OrderStation]: WaitSpot is null.", this);

        if (ExitSpot == null)
            Debug.LogError("[OrderStation]: ExitSpot is null.", this);

        if (AvailableMenuItems == null)
        {
            Debug.LogError("[OrderStation]: AvailableMenuItems is null.", this);
            return;
        }

        for (int i = 0; i < AvailableMenuItems.Count; i++)
        {
            if (AvailableMenuItems[i] == null)
                Debug.LogError($"[OrderStation]: AvailableMenuItems[{i}] is null.", this);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Draw OrderSpot
        if (OrderSpot != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(OrderSpot.position, 0.3f);
            Gizmos.DrawLine(transform.position, OrderSpot.position);
        }
        if (OrderLineEntryPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(OrderLineEntryPoint.position, 0.3f);
            Gizmos.DrawLine(transform.position, OrderLineEntryPoint.position);
        }

        // Draw WaitSpot
        if (WaitSpot != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(WaitSpot.position, 0.3f);
            Gizmos.DrawLine(transform.position, WaitSpot.position);
        }

        // Draw ExitSpot
        if (ExitSpot != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(ExitSpot.position, 0.3f);
            Gizmos.DrawLine(transform.position, ExitSpot.position);
        }

        // Draw OrderSubmissionArea (approximate)
        if (OrderSubmissionArea != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(OrderSubmissionArea.transform.position, Vector3.one * 0.75f);
            Gizmos.DrawLine(transform.position, OrderSubmissionArea.transform.position);
        }

        // Draw OrderLine root
        if (OrderLine != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(OrderLine.transform.position, 0.5f);
            Gizmos.DrawLine(transform.position, OrderLine.transform.position);
        }
    }
}