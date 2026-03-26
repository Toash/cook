using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI-facing controller for building the player's current supply cart/order.
/// The UI should call into this instead of mutating SupplyOrder directly.
/// </summary>
public class SupplyOrderController : MonoBehaviour
{
    public static SupplyOrderController I { get; private set; }

    [SerializeField] private SupplyOrder currentCart = new();

    public SupplyOrder CurrentCart => currentCart;

    /// <summary>
    /// Fired whenever the contents of the cart change.
    /// UI can subscribe and refresh itself.
    /// </summary>
    public event Action<SupplyOrder> CartChanged;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
    }

    public List<SupplyEntry> GetEntries()
    {
        return currentCart.Items;
    }

    public int GetAmount(HoldableData item)
    {
        return currentCart.GetAmount(item);
    }

    public bool Contains(HoldableData item)
    {
        return GetAmount(item) > 0;
    }

    public void AddItem(HoldableData item, int amount = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("[SupplyOrderController]: AddItem called with null item.");
            return;
        }

        if (amount <= 0) return;

        currentCart.AddAmount(item, amount);
        NotifyCartChanged();
    }

    public void RemoveItem(HoldableData item, int amount = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("[SupplyOrderController]: RemoveItem called with null item.");
            return;
        }

        if (amount <= 0) return;

        currentCart.RemoveAmount(item, amount);
        NotifyCartChanged();
    }

    public void SetItemAmount(HoldableData item, int amount)
    {
        if (item == null)
        {
            Debug.LogWarning("[SupplyOrderController]: SetItemAmount called with null item.");
            return;
        }

        currentCart.SetAmount(item, amount);
        NotifyCartChanged();
    }

    public void RemoveItemCompletely(HoldableData item)
    {
        if (item == null) return;

        currentCart.SetAmount(item, 0);
        NotifyCartChanged();
    }

    public void ClearCart()
    {
        currentCart.Clear();
        NotifyCartChanged();
    }

    public bool HasItems()
    {
        return !currentCart.IsEmpty();
    }

    /// <summary>
    /// Returns a shallow copy of the current cart so another system can consume it safely.
    /// </summary>
    public SupplyOrder CreateOrderSnapshot()
    {
        SupplyOrder copy = new SupplyOrder();

        foreach (var entry in currentCart.Items)
        {
            if (entry == null || entry.Item == null || entry.Amount <= 0) continue;

            copy.Items.Add(new SupplyEntry
            {
                Item = entry.Item,
                Amount = entry.Amount
            });
        }

        return copy;
    }

    private void NotifyCartChanged()
    {
        CartChanged?.Invoke(currentCart);
    }
}