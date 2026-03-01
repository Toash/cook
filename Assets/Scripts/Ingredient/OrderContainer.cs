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

    void Awake()
    {
        snapper = GetComponent<Snapper>();
        snapper.SetJointPriority(JointPriority.Container);
        snapper.SetJointType(JointType.Container);
    }
    void OnEnable()
    {
        snapper.OnSnapEvent += OnSnap;
        snapper.OnDetachedEvent += DetachPreparedItem;
    }
    void OnDisable()
    {
        snapper.OnSnapEvent -= OnSnap;
        snapper.OnDetachedEvent -= DetachPreparedItem;
    }


    public bool CanSubmit()
    {
        return Receipt != null;
    }


    void OnSnap(SnapConnection connection)
    {
        if (connection.Other.TryGetComponent<Ingredient>(out var _))
        {
            // the other snapper is an ingredient, make a prepared item.
            CreatePreparedItem(connection);
        }
        else if (connection.Other.TryGetComponent<OrderReceipt>(out var receipt))
        {
            LinkOrder(receipt);
        }
    }

    void OnDetached(SnapConnection connection)
    {
        if (connection.Other.TryGetComponent<Ingredient>(out var _))
        {
            // the other snapper is an ingredient, make a prepared item.
            DetachPreparedItem(connection);
        }
        else if (connection.Other.TryGetComponent<OrderReceipt>(out var receipt))
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
    void CreatePreparedItem(SnapConnection connection)
    {
        List<Ingredient> ingredients = new();

        List<Snapper> connectedSnappers = connection.Other.GetSnapperGroup();
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
    void DetachPreparedItem(SnapConnection connection)
    {
        // get prepared item from the other and remove that
        if (connection.Other.TryGetComponent<Ingredient>(out var ingredient))
        {
            PreparedItem preparedItem = ingredient.PreparedItem;
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
    void OnDrawGizmos()
    {
        Handles.Label(transform.position, "Prepared Item Count: " + PreparedItems.Count.ToString());

        if (Receipt != null)
        {
            style.normal.textColor = Color.blue;
            style.fontStyle = FontStyle.Bold;
            Handles.Label(transform.position + (Vector3.up * .2f), "Associated Order ID:" + Receipt.OrderID);
        }



    }
#endif
}