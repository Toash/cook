
using System.Collections.Generic;
using Assets.Scripts.Ingredient.MenuItem;

/// <summary>
/// Represents an order that an npc is proposing.
/// </summary>
[System.Serializable]
public class OrderProposition
{
    public NPC Proposer;
    public List<OrderMenuItem> MenuItems;
    public int Payout;

    // orders should be created through order manager.
    public OrderProposition(NPC proposer, List<OrderMenuItem> items, int payout)
    {
        this.Proposer = proposer;
        this.MenuItems = items;
        this.Payout = payout;
    }
}


