

using System.Collections.Generic;
using UnityEditor;
using UnityEngine.InputSystem.Android;
using Assets.Scripts.Ingredient.MenuItem;


/// <summary>
/// Represents an order
/// Customer can place an order in restaurant
///
/// Customer can place order in drivethrough
/// Customer can place order over phone 
/// 
/// Submission areas look for order. 
/// </summary>
public class Order
{
    public NPC Owner;
    public List<Assets.Scripts.Ingredient.MenuItem.OrderMenuItem> MenuItems;
    public int Payout;


    public static int Counter = 1;
    public int ID;
    // long order time bad.
    public float TimeSinceOrdered;


    // orders should be created through order manager.
    // public Order(List<MenuItem> items, int Payout)
    // {
    //     OrderNumber = Counter;
    //     Counter++;

    //     this.MenuItems = items;
    //     this.Payout = Payout;
    // }
    public Order(OrderProposition proposition)
    {
        ID = Counter;
        Counter++;

        this.MenuItems = proposition.MenuItems;
        this.Payout = proposition.Payout;
        this.Owner = proposition.Proposer;
    }



}



