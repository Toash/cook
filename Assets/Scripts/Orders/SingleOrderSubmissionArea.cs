using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Auto submits when receciving an OrderContainer that is linked to an active order.  <br/>
/// Is also the place where npc picks up an order.
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


    void OnSnap(Snapper otherSnapper)
    {
        var container = otherSnapper.GetComponent<OrderContainer>();
        if (container != null)
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
            Debug.Log("[OrderSubmissionArea]: Failed to submit PreparedItems for an Order. Status was not Success");
        }
    }


    /// <summary>
    /// Gives a reference to the container if it is linked to the orderid.
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

    }

#endif

}