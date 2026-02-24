

using System.Collections.Generic;

/// <summary>
/// Represents an order
/// Customer can place an order in restaurant
///
/// Customer can place order in drivethrough
/// Customer can place order over phone 
/// 
/// Submission areas look for order. 
/// </summary>
[System.Serializable]
public class Order
{
    public static int Counter = 1;
    public int OrderNumber;
    public List<MenuItem> MenuItems;

    public int Payout;



}



