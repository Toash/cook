using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Place to submit an order
/// 
/// </summary>
[RequireComponent(typeof(Snapper))]
public class SingleOrderSubmissionArea : MonoBehaviour
{
    [ReadOnly, Tooltip("The current container that is in the area.")]
    public OrderContainer CurrentContainer = null;
    [Tooltip("Where NPC will go to pickup their order.")]
    public Transform PickupSpot;

    public UnityEvent OnOrderSuccessful;
    public UnityEvent OnOrderFailed;


    private Player player;
    private Snapper snapper;


    void Awake()
    {
        snapper = GetComponent<Snapper>();

    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("[OrderSubmissionArea]: Could not find player");
        }
    }


    void OnEnable()
    {
        snapper.OnChildSnapped += OnSnap;
    }
    void OnDisable()
    {
        snapper.OnChildSnapped -= OnSnap;
    }


    void OnSnap(Snapper otherSnapper)
    {
        if (otherSnapper.TryGetComponent<OrderContainer>(out var container))
        {
            OnContainerAdded(container);

        }
        else
        {
            Debug.LogWarning("[OrderSubmissionArea]: OrderSubmissionArea expects a container");

        }
    }
    void OnContainerAdded(OrderContainer container)
    {
        Debug.Log("[OrderSubmissionArea]: Container Entered Submission Area");

        CurrentContainer = container;
        TrySubmitCurrentContainer();
        CurrentContainer.ReceiptSnapped += TrySubmitCurrentContainer;
        CurrentContainer.PreparedItemUpdated += TrySubmitCurrentContainer;
        // check if can submit
        // if (container.CanSubmit())
        // {
        //     SubmitContainer(container);
        // }
    }


    public void TrySubmitCurrentContainer()
    {
        Debug.Log("[OrderSubmissionArea]: Trying to submit current container");

        CurrentContainer.TryGetLinkedOrder(out Order order);
        CurrentContainer.TryGetPreparedItems(out List<PreparedItem> items);

        OrderSubmissionResult result = OrderManager.I.PlayerTrySubmit(order, items);

        // if (result.Status != OrderSubmissionStatus.Success)
        // {
        //     Debug.Log("[OrderSubmissionArea]: Failed to submit PreparedItems for an Order. Status was not Success");
        // }
    }


    /// <summary>
    /// Gives a reference to the container if it is linked to the orderid. <br/>
    /// 
    /// Sets current container reference to null. 
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    public bool TryTakeContainer(int orderId, out OrderContainer container)
    {
        if (orderId != CurrentContainer.GetOrderID())
        {
            container = null;
            return false;
        }

        container = CurrentContainer;

        CurrentContainer.Snapper.DetachFromParent();
        CurrentContainer.ReceiptSnapped -= TrySubmitCurrentContainer;
        CurrentContainer.PreparedItemUpdated -= TrySubmitCurrentContainer;

        CurrentContainer = null;
        return true;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.orange;

        string message = "";
        message += "Submission Area";
        Handles.Label(transform.position, message);


        if (CurrentContainer != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, .2f);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, .2f);

        }

    }

#endif

}