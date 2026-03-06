using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Source of truth for an order submission<br/>
/// Holds all attached (snapped) Prepared Items<br/>
/// Holds a receipt that associates it when an active order.
/// <br/>
/// </summary>
[RequireComponent(typeof(Snapper))]
public class OrderContainer : MonoBehaviour
{

    // prepared items that are connected with this container
    public List<PreparedItem> PreparedItems = new();

    public OrderReceipt Receipt;
    private Snapper snapper;
    private PhysicsGrabbable grabbable;

    void Awake()
    {
        grabbable = GetComponent<PhysicsGrabbable>();
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


    public bool CanSubmit()
    {
        bool hasPreparedItems = PreparedItems.Count > 0;
        bool hasReceipt = Receipt != null;

        if (!hasReceipt)
        {
            Debug.LogError("[OrderContainer]: Cannot submit, does not have receipt");
        }

        if (!hasPreparedItems)
        {
            Debug.LogError("[OrderContainer]: Cannot submit, does not have atleast 1 prepared item.");
        }


        return hasReceipt && hasPreparedItems;
    }

    public Order GetLinkedOrder()
    {
        if (Receipt == null)
        {
            Debug.LogError("[OrderContainer]: Could not get linked order. Receipt is null.");
            return null;
        }

        var order = OrderManager.I.GetActiveOrderFromID(Receipt.OrderID);
        return order;
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
            CreatePreparedItem(otherSnapper);
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
    void CreatePreparedItem(Snapper otherSnapper)
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
        // item.transform.SetParent(gameObject.transform);

        Debug.Log("[OrderContainer]: Created prepared item on container");
        PreparedItems.Add(item);
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
            PreparedItems.Remove(preparedItem);

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
        message += "Prepared Items Count: " + PreparedItems.Count + "\n";
        Handles.Label(transform.position, message, style);
        foreach (var item in PreparedItems)
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