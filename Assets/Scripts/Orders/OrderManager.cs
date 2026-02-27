using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Keeps track of orders and manages them
/// </summary>
public class OrderManager : MonoBehaviour
{
    public static OrderManager I;
    public List<Order> Orders = new List<Order>();


    public event Action<Order> OrderAdded;
    public event Action<Order> OrderRemoved;




    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        else if (I == null)
        {
            DontDestroyOnLoad(gameObject);
            I = this;
        }
    }



    public void Update()
    {
        foreach (var order in Orders)
        {
            order.TimeSinceOrdered += Time.deltaTime;
        }
    }

    /// <summary>
    /// Gets a random order
    /// 
    /// TODO dynamically calculate the order based on stuff 
    /// </summary>
    /// <returns></returns>
    public Order AddRandomOrder()
    {
        List<MenuItem> menuItems = new List<MenuItem>();


        // probably get this based on some difficulty thing
        int menuItemCount = 1;

        for (int i = 0; i < menuItemCount; i++)
        {
            MenuItem item = MenuItem.GetRandomMenuItem();
            menuItems.Add(item);
        }

        // calculate payout based on difficulty 
        Order order = new Order(menuItems, 100);
        return AddOrder(order);
    }

    public OrderSubmissionResult TrySubmit(Order order, List<PreparedItem> preparedItems)
    {
        Debug.Log("[OrderManager]: Received Order...");
        if (order == null)
        {
            Debug.Log("[OrderManager]: No order available");
            return new OrderSubmissionResult(OrderSubmissionStatus.NoOrder, null, null, 0);
        }

        // evaluate order
        Debug.Log("[OrderManager]: Evaluating order...");
        List<PreparedItemData> itemData = PreparedItemData.From(preparedItems);
        OrderEvaluationResult eval = OrderEvaluator.Evaluate(order, itemData);


        //TODO payout calculation
        float payout = order.Payout;

        MoneyManager.I.AddMoney(payout);
        Debug.Log("[OrderManager]: Returning order information.");
        return new OrderSubmissionResult(OrderSubmissionStatus.Success, order, eval, payout);

    }


    public Order AddOrder(Order order)
    {
        Orders.Add(order);
        OrderAdded?.Invoke(order);
        return order;
    }

    public void RemoveOrder(Order order)
    {
        if (!Orders.Remove(order)) return;
        OrderRemoved?.Invoke(order);
    }


}