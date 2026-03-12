using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Source of truth for an order submission<br/>
/// Holds all attached (snapped) Prepared Items<br/>
/// Holds a receipt that associates it when an active order.
/// <br/>
/// </summary>
[RequireComponent(typeof(Snapper))]
[RequireComponent(typeof(Holdable))]
public class OrderContainer : MonoBehaviour
{

    // prepared items that are connected with this container
    public List<PreparedItem> ContainedPreparedItems = new();

    public OrderReceipt Receipt;
    [ReadOnly, Tooltip("Gets set when the container is submitted")]
    public OrderSubmissionResult SubmissionResult;
    private Snapper snapper;

    public event Action PreparedItemUpdated; // indicates that a prepared item was updated inside of this order container.
    public event Action ReceiptSnapped;


    void OnValidate()
    {
        if (TryGetComponent<Collider>(out var collider))
        {
            collider.isTrigger = true;
        }

    }
    void Awake()
    {
        snapper = GetComponent<Snapper>();
        snapper.SetSnapType(SnapType.Container);
    }
    void OnEnable()
    {
        snapper.OnChildSnapped += OnSnap;
        snapper.OnChildDetached += OnDetached;
    }
    void OnDisable()
    {
        snapper.OnChildSnapped -= OnSnap;
        snapper.OnChildDetached -= OnDetached;
    }


    // public bool CanSubmit()
    // {
    //     bool hasPreparedItems = PreparedItems.Count > 0;
    //     bool hasReceipt = Receipt != null;

    //     if (!hasReceipt)
    //     {
    //         Debug.Log("[OrderContainer]: Cannot submit, does not have receipt");
    //     }

    //     if (!hasPreparedItems)
    //     {
    //         Debug.Log("[OrderContainer]: Cannot submit, does not have atleast 1 prepared item.");
    //     }


    //     return hasReceipt && hasPreparedItems;
    // }

    public bool TryGetPreparedItems(out List<PreparedItem> items)
    {
        if (ContainedPreparedItems.Count > 0)
        {
            items = ContainedPreparedItems;
            return true;
        }

        items = null;
        return false;

    }
    public bool TryGetLinkedOrder(out Order order)
    {
        if (Receipt == null)
        {
            Debug.Log("[OrderContainer]: Could not get linked order. Receipt is null.");
            order = null;
            return false;
        }

        order = OrderManager.I.GetActiveOrderFromID(Receipt.OrderID);
        return true;
    }

    public int GetOrderID()
    {
        return Receipt.OrderID;
    }


    void OnSnap(Snapper otherSnapper)
    {
        if (otherSnapper.TryGetComponent<Ingredient>(out var _))
        {
            // the other snapper is an ingredient, make a prepared item.
            CreateContainedPreparedItem(otherSnapper);
        }
        else if (otherSnapper.TryGetComponent<OrderReceipt>(out var receipt))
        {
            LinkOrder(receipt);
        }
    }

    void OnDetached(Snapper otherSnapper)
    {
        if (otherSnapper.TryGetComponent<Ingredient>(out var _))
        {
            DetachPreparedItem(otherSnapper);
        }
        else if (otherSnapper.TryGetComponent<OrderReceipt>(out var receipt))
        {
            UnlinkOrder(receipt);
        }
    }

    /// <summary>
    /// Associate this container with an OrderReceipt so it can be linked to an order later on.
    /// </summary>
    /// <param name="receipt"></param>
    void LinkOrder(OrderReceipt receipt)
    {
        this.Receipt = receipt;
        ReceiptSnapped?.Invoke();
    }
    void UnlinkOrder(OrderReceipt receipt)
    {
        if (receipt != this.Receipt)
        {
            Debug.LogError("[OrderContainer]: The same receipt is not set in the OrderContainer but it is trying to be detached.");
        }
        this.Receipt = null;
    }


    /// <summary>
    /// Create prepared item on order container.
    /// </summary>
    /// <param name="connection"></param>
    void CreateContainedPreparedItem(Snapper otherSnapper)
    {

        List<Ingredient> ingredients = new();

        List<Snapper> connectedSnappers = otherSnapper.GetSnapperChildrenRecursive();
        foreach (var snapper in connectedSnappers)
        {
            if (snapper.TryGetComponent<Ingredient>(out var ingredient))
            {
                if (ingredient.PreparedItem != null) continue;
                Debug.Log("[OrderContainer]: Creating prepared item with ingredient: " + ingredient);
                ingredients.Add(ingredient);
            }
        }

        PreparedItem item = PreparedItem.Create(ingredients);
        item.IngredientsChanged += PreparedItemUpdated;
        // item.transform.SetParent(gameObject.transform);

        Debug.Log("[OrderContainer]: Created prepared item on container");
        ContainedPreparedItems.Add(item);
    }

    /// <summary>
    /// Detach prepraed item from order container.
    /// </summary>
    /// <param name="connection"></param>
    void DetachPreparedItem(Snapper otherSnapper)
    {
        if (otherSnapper.TryGetComponent<Ingredient>(out var ingredient))
        {
            PreparedItem preparedItem = ingredient.PreparedItem;
            if (preparedItem == null)
            {
                Debug.LogError("[OrderContainer]: PreparedItem was null when trying to detach ingredient");
            }
            // preparedItem.transform.SetParent(null);
            preparedItem.IngredientsChanged -= PreparedItemUpdated;
            ContainedPreparedItems.Remove(preparedItem);

            preparedItem.Disband();

            Debug.Log("[PreparedItemsContainer]: Disbanded prepared item on container");
        }
    }



    public static bool ContainsContainerInGroup(List<Snapper> snappers)
    {
        foreach (var snapper in snappers)
        {
            if (snapper.GetComponent<OrderContainer>() != null)
            {
                return true;
            }
        }
        return false;
    }



#if UNITY_EDITOR
    GUIStyle style = new();
    void OnDrawGizmosSelected()
    {
        style.normal.textColor = Color.blue;
        style.fontStyle = FontStyle.Bold;
        string message = "";
        message += "Prepared Items Count: " + ContainedPreparedItems.Count + "\n";
        Handles.Label(transform.position, message, style);
        foreach (var item in ContainedPreparedItems)
        {
            message += item.ToString() + "\n";
            Handles.Label(transform.position + (Vector3.up * .2f), message, style);
        }

        if (Receipt != null)
        {
            message += "Associated Order ID:" + Receipt.OrderID + "\n";
            Handles.Label(transform.position + (Vector3.up * .3f), message, style);
        }



    }
#endif
}