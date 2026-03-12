using System;
using System.Collections.Generic;
using Assets.Scripts.Ingredient.MenuItem;
using Sirenix.OdinInspector;
using UnityEngine;
public class OrderManager : MonoBehaviour
{
    public static OrderManager I;




    /// <summary>
    /// Order that the player has to acknowledge before it gets added to the active orders.
    /// </summary>
    [ReadOnly]
    public OrderProposition ProposedOrder = null;

    // TODO: add way for player to reject order (out of stock), or counter offer.
    public event Action<Order> ProposedOrderAcknowledged;
    public event Action<OrderProposition> ProposedOrderAdded;
    public event Action<OrderProposition> ProposedOrderRemoved;


    /// <summary>
    /// Orders that the player has to currently make.
    /// </summary>
    public List<Order> ActiveOrders = new List<Order>();
    public event Action<Order> ActiveOrderSuccessfullySubmitted;
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

        Debug.Log(HasProposedOrder());
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

    /// <summary>
    /// Generates a random order proposition from an npc
    /// </summary>
    /// <param name="proposer"></param>
    /// <returns></returns>
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
    public bool HasProposedOrder()
    {
        return ProposedOrder != null;

    }

    public void ProposeOrder(OrderProposition prop)
    {
        // if (ProposedOrder != null)
        // {
        //     Debug.LogWarning("[OrderManager]: Trying to propose Order but it already exists..." + ProposedOrder);
        //     return;
        // }
        Debug.Log("[OrderManager]: Proposed order");
        ProposedOrder = prop;
        ProposedOrderAdded?.Invoke(prop);
    }

    /// <summary>
    /// Acknowledge a proposed order, turning it into an active order to be made
    /// </summary>
    public void AcknowledgeProposedOrder()
    {
        if (ProposedOrder == null)
        {
            Debug.LogError("[OrderManager]: No proposed order to acknowledge");

        }
        Debug.Log("[OrderManager]: Acknowledged proposed order");
        ProposedOrderRemoved?.Invoke(ProposedOrder);


        Order newOrder = new Order(ProposedOrder);

        AddToActiveOrders(newOrder);
        ProposedOrder = null;


        ProposedOrderAcknowledged?.Invoke(newOrder);
    }

    /// <summary>
    /// Tries to submit an order. Can be successful or not.
    /// </summary>
    /// <param name="order"></param>
    /// <param name="preparedItems"></param>
    /// <returns></returns>
    public OrderSubmissionResult TrySubmit(Order order, List<PreparedItem> preparedItems)
    {
        Debug.Log("[OrderManager]: Received Order...");
        if (order == null)
        {
            Debug.Log("[OrderManager]: No order available");
            return new OrderSubmissionResult(OrderSubmissionStatus.NoOrder, null, null, 0);
        }

        if (preparedItems == null || preparedItems.Count == 0)
        {
            Debug.Log("[OrderManager]: No prepared items in order");
            return new OrderSubmissionResult(OrderSubmissionStatus.NoPreparedItems, null, null, 0);
        }

        // evaluate order
        Debug.Log("[OrderManager]: Evaluating order...");
        List<PreparedItemData> itemsData = PreparedItemData.From(preparedItems);
        OrderEvaluationResult eval = OrderEvaluator.Evaluate(order, itemsData);


        //TODO payout calculation
        float payout = order.Payout;

        MoneyManager.I.ChangeMoney(payout);
        Debug.Log("[OrderManager]: Returning order information.");


        // remove from active orders.
        RemoveActiveOrder(order);
        ActiveOrderSuccessfullySubmitted?.Invoke(order);

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