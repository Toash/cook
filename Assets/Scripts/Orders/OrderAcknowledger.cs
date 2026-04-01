using UnityEngine;
public class OrderAcknowledger : InteractableBase
{

    protected override void Start()
    {
        base.Start();
        HoverTooltipData = new HoverTooltipData(transform, "No Order");
        OrderManager.I.ProposedOrderAdded += OnProposedOrderAdded;
        OrderManager.I.ProposedOrderRemoved += OnProposedOrderRemoved;
    }
    void OnDestroy()
    {
        OrderManager.I.ProposedOrderAdded -= OnProposedOrderAdded;
        OrderManager.I.ProposedOrderRemoved -= OnProposedOrderRemoved;
    }
    public override void Interact(InteractionContext context)
    {
        if (OrderManager.I.HasProposedOrder())
        {
            OrderManager.I.AcknowledgeProposedOrder();
        }
    }
    void OnProposedOrderAdded(OrderProposition orderProposition)
    {
        HoverTooltipData = new HoverTooltipData(transform, "Accept");

    }
    void OnProposedOrderRemoved(OrderProposition orderProposition)
    {
        HoverTooltipData = new HoverTooltipData(transform, "No Order");

    }
}