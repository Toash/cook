using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupplyCartUI : MonoBehaviour
{
    public SupplyOrderController Controller;

    [Header("UI")]
    public Transform Container;
    public SupplyCartItemUI ItemPrefab;

    [Header("Buttons")]
    public Button OrderButton;
    public Button ClearButton;

    // [Header("Order Settings")]
    // public int DeliveryDelaySeconds = 300;

    private readonly List<SupplyCartItemUI> activeItems = new();

    private void OnEnable()
    {
        if (Controller != null)
        {
            Controller.CartChanged += OnCartChanged;
        }

        if (OrderButton != null)
        {
            OrderButton.onClick.AddListener(OnOrderClicked);
        }

        if (ClearButton != null)
        {
            ClearButton.onClick.AddListener(OnClearClicked);
        }
    }

    private void OnDisable()
    {
        if (Controller != null)
        {
            Controller.CartChanged -= OnCartChanged;
        }

        if (OrderButton != null)
        {
            OrderButton.onClick.RemoveListener(OnOrderClicked);
        }

        if (ClearButton != null)
        {
            ClearButton.onClick.RemoveListener(OnClearClicked);
        }
    }

    private void Start()
    {
        if (Controller != null)
        {
            OnCartChanged(Controller.CurrentCart);
        }
    }

    private void OnCartChanged(SupplyOrder order)
    {

        ClearRows();

        foreach (var entry in order.Items)
        {
            if (entry == null || entry.Item == null || entry.Amount <= 0) continue;

            var ui = Instantiate(ItemPrefab, Container);
            ui.Init(entry.Item, entry.Amount);
            activeItems.Add(ui);
        }

        RefreshButtonState(order);
    }

    private void OnOrderClicked()
    {
        if (Controller == null) return;
        if (DeliveryManager.I == null) return;
        if (!Controller.HasItems()) return;

        SupplyOrder snapshot = Controller.CreateOrderSnapshot();
        DeliveryManager.I.OrderSupplies(snapshot);
        Controller.ClearCart();
    }

    private void OnClearClicked()
    {

        Controller.ClearCart();
    }

    private void ClearRows()
    {
        foreach (var item in activeItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }

        activeItems.Clear();
    }

    private void RefreshButtonState(SupplyOrder order)
    {
        bool hasItems = order != null && !order.IsEmpty();

        OrderButton.interactable = hasItems;

        ClearButton.interactable = hasItems;
    }
}