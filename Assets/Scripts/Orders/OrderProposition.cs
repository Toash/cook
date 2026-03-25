
using System.Collections.Generic;

/// <summary>
/// Represents an order that an npc is proposing.
/// </summary>
[System.Serializable]
public class OrderProposition
{
    public NPC Proposer;
    public List<OrderedMenuItem> Items;
    public int Payout;

    // orders should be created through order manager.
    public OrderProposition(NPC proposer, List<OrderedMenuItem> items, int payout)
    {
        this.Proposer = proposer;
        this.Items = items;
        this.Payout = payout;
    }
}


