using System.Collections.Generic;

public class Order
{
    public NPC Owner;
    public List<OrderedMenuItem> Items;
    public int Payout;

    public static int Counter = 1;
    public int ID;
    public float TimeSinceOrdered;

    public Order(OrderProposition proposition)
    {
        ID = Counter++;
        Payout = proposition.Payout;
        Owner = proposition.Proposer;
        Items = proposition.Items;
    }
}