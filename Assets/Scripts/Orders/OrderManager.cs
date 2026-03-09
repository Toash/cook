using System;
using System.Collections.Generic;
using Assets.Scripts.Ingredient.MenuItem;
using UnityEngine;
public class OrderManager : MonoBehaviour
{
    public static OrderManager I;




    /// <summary>
    /// Order that the player has to acknowledge before it gets added to the active orders.
    /// </summary>
    public OrderProposition ProposedOrder = null;

    // TODO: add way for player to reject order (out of stock), or counter offer.
    public event Action<Order> ProposedOrderAcknowledged;
    public event Action<OrderProposition> ProposedOrderAdded;
    public event Action<OrderProposition> ProposedOrderRemoved;


    /// <summary>
    /// Orders that the player has to currently make.
    /// </summary>
    public List<Order> ActiveOrders = new List<Order>();
    public event Action<Order> ActiveOrderMade;
    public event Action<Order> ActiveOrderAdded;
    public event Action<Order> ActiveOrderRemoved;




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
        foreach (var order in ActiveOrders)
        {
            order.TimeSinceOrdered += Time.deltaTime;
        }
    }

    public Order GetActiveOrderFromID(int ID)
    {
        foreach (var order in ActiveOrders)
        {
            if (order.ID == ID)
            {
                return order;
            }
        }

        Debug.Log("[OrderManager]: Could not find associated order from ID " + ID + "!");
        return null;

    }

    public OrderProposition GenerateRandomOrderProposition(NPC proposer)
    {
        List<MenuItem> menuItems = new List<MenuItem>();

        // probably get this based on some difficulty thing
        int menuItemCount = 1;

        for (int i = 0; i < menuItemCount; i++)
        {
            MenuItem item = MenuItem.GetRandomMenuItem();
            menuItems.Add(item);
        }

        // TODO calculate price based on menu items
        OrderProposition order = new OrderProposition(proposer, menuItems, 100);
        return order;
        // return AddToActiveOrders(order);
    }

    public void ProposeOrder(OrderProposition prop)
    {
        if (ProposedOrder != null)
        {
            Debug.LogWarning("[OrderManager]: Trying to propose Order but it already exists...");
            return;
        }
        Debug.Log("[OrderManager]: Proposed order");
        ProposedOrder = prop;
        ProposedOrderAdded?.Invoke(prop);
    }

    public void AcknowledgeProposedOrder()
    {
        Debug.Log("[OrderManager]: Acknowledged proposed order");
        ProposedOrderRemoved?.Invoke(ProposedOrder);


        Order newOrder = new Order(ProposedOrder);

        AddToActiveOrders(newOrder);
        ProposedOrder = null;


        ProposedOrderAcknowledged?.Invoke(newOrder);
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

        MoneyManager.I.ChangeMoney(payout);
        Debug.Log("[OrderManager]: Returning order information.");


        // remove from active orders.
        RemoveActiveOrder(order);
        ActiveOrderMade?.Invoke(order);

        return new OrderSubmissionResult(OrderSubmissionStatus.Success, order, eval, payout);

    }


    public Order AddToActiveOrders(Order order)
    {
        ActiveOrders.Add(order);
        ActiveOrderAdded?.Invoke(order);
        return order;
    }

    public void RemoveActiveOrder(Order order)
    {
        if (!ActiveOrders.Remove(order)) return;
        ActiveOrderRemoved?.Invoke(order);
    }


}