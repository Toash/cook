using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Auto submits when receciving an OrderContainer that is linked to an active order.  <br/>
/// Is also the place where npc picks up an order.
/// </summary>
[RequireComponent(typeof(Collider))]
public class SingleOrderSubmissionArea : MonoBehaviour
{
    [ReadOnly, Tooltip("The current container that is in the area.")]
    public OrderContainer CurrentContainer = null;
    [Tooltip("Where to snap the container to when sbumitting.")]
    public Transform OrderContainerSnap;
    [Tooltip("Where NPC will go to pickup their order.")]
    public Transform PickupSpot;

    public UnityEvent OnOrderSuccessful;
    public UnityEvent OnOrderFailed;


    private Collider col;
    private Player player;


    void Awake()
    {
        if (OrderContainerSnap == null)
        {
            Debug.LogError("Could not find OrderContainerSnap");
        }
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("[OrderSubmissionArea]: Could not find player");
        }
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
            CurrentContainer = container;
            SubmitContainer(container);
        }
    }


    public void SubmitContainer(OrderContainer container)
    {
        Order order = container.GetLinkedOrder();

        OrderSubmissionResult result = OrderManager.I.TrySubmit(order, container.PreparedItems);

        if (result.Status != OrderSubmissionStatus.Success)
        {
            Debug.Log("[OrderSubmissionArea]: Failed to submit PreparedItems for an Order.");
        }

        // stop player from grabbing
        player.Grabber.Drop();

        // Freeze physics on the ordercontainer
        container.Freeze();


        //snap order to location
        container.transform.position = OrderContainerSnap.position;
        container.transform.rotation = OrderContainerSnap.rotation;
    }


    public bool TryPickup(int orderId, Transform handSocket, out OrderContainer container)
    {
        if (orderId != CurrentContainer.GetOrderID())
        {
            container = null;
            return false;
        }

        container = CurrentContainer;

        CurrentContainer = null;
        return true;
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

        if (OrderContainerSnap != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(OrderContainerSnap.transform.position, .3f);
        }
    }

#endif

}