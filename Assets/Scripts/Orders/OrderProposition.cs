
using System.Collections.Generic;
using Assets.Scripts.Ingredient.MenuItem;

public class OrderProposition
{
    public NPC Proposer;
    public List<MenuItem> MenuItems;
    public int Payout;

    // orders should be created through order manager.
    public OrderProposition(NPC proposer, List<MenuItem> items, int payout)
    {
        this.Proposer = proposer;
        this.MenuItems = items;
        this.Payout = payout;
    }
}


