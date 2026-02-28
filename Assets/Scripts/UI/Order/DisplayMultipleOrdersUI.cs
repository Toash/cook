
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Displays all the orders from the order manager, based on its events
/// </summary>
public class DisplayMultipleOrdersUI : MonoBehaviour
{
    public OrderManager OrderManager;
    public Transform OrdersFlexContainer;
    public DisplaySingleOrderUI DisplaySingleOrderUI;
    private List<Order> orders;

    void OnEnable()
    {
        OrderManager.ActiveOrderAdded += OnAddOrder;
        OrderManager.ActiveOrderRemoved += OnRemoveOrder;
    }
    void OnDisable()
    {
        OrderManager.ActiveOrderAdded -= OnAddOrder;
        OrderManager.ActiveOrderRemoved -= OnRemoveOrder;
    }


    void OnAddOrder(Order order)
    {
        ClearFlexContainer();
        Populate(OrderManager.I.ActiveOrders);

    }
    void OnRemoveOrder(Order order)
    {
        ClearFlexContainer();
        Populate(OrderManager.I.ActiveOrders);

    }


    void ClearFlexContainer()
    {
        for (int i = OrdersFlexContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(OrdersFlexContainer.GetChild(i).gameObject);
        }

    }

    public void Populate(List<Order> orders)
    {
        this.orders = orders;

        foreach (var order in orders)
        {
            var orderUI = Instantiate(DisplaySingleOrderUI, OrdersFlexContainer);
            orderUI.Populate(order);
        }



    }


}