using UnityEngine;

/// <summary>
/// Gives holdables on interaction
/// </summary>
public class ContainerDispensor : InteractableBase
{
    public HoldableData ContainedItem;
    Holdable containedHoldable;
    void Start()
    {
        containedHoldable = ContainedItem.Prefab;
        UpdateTooltip();

    }

    public override void Interact(InteractionContext context)
    {
        if (context.Type == InteractType.Secondary)
        {
            Debug.Log("Secondary");
            Pickup(context);
        }

    }
    void Pickup(InteractionContext context)
    {

        Holdable holdable = Instantiate(containedHoldable);
        context.Player.ItemHolder.TryHold(holdable);


        UpdateTooltip();
    }

    void UpdateTooltip()
    {
        string message = ContainedItem.Name;
        var data = new HoverTooltipData(transform, message);
        HoverTooltipData = data;
    }
}