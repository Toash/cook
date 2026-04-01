using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A refillable container/tray that stores items loaded from a HoldableContainer.
/// Can:
/// - be picked up
/// - dispense one item to the player while in the world
/// - accept items from a compatible held HoldableContainer
/// </summary>
[RequireComponent(typeof(ItemContainer))]
public class BulkSourceContainer : Holdable
{
    [Header("Container")]
    public ItemContainer Container;

    [Header("Dispense")]
    public InteractType DispenseToPlayerInteractType = InteractType.Secondary;
    public bool AllowDispenseToPlayer = true;

    [Header("UI")]
    public List<InteractInfo> EmptyHandInteractInfoCanTake = new()
    {
        new InteractInfo(InteractType.Primary, "Pickup"),
        new InteractInfo(InteractType.Secondary, "Take item")
    };

    public List<InteractInfo> EmptyHandInteractInfoCantTake = new()
    {
        new InteractInfo(InteractType.Primary, "Pickup"),
    };

    public List<InteractInfo> WhileHoldingContainerInfo = new()
    {
        new InteractInfo(InteractType.Primary, "Load tray")
    };

    protected override void OnValidate()
    {
        base.OnValidate();

        if (Container == null)
            Container = GetComponent<ItemContainer>();
    }

    protected override void Start()
    {
        base.Start();

        if (Container == null)
            Container = GetComponent<ItemContainer>();
    }

    public bool CanAccept(HoldableContainer source)
    {
        if (source == null || source.Container == null || Container == null)
            return false;

        if (Container.ContainedItemData == null)
            return false;

        if (!source.Container.CanProvide(Container.ContainedItemData))
            return false;

        return Container.CurrentAmount < Container.Capacity;
    }

    public int AddItems(int amount)
    {
        if (Container == null)
            return 0;

        return Container.AddItems(amount);
    }

    public bool TryRemoveOne()
    {
        return Container != null && Container.TryRemoveOne();
    }

    public override List<InteractInfo> GetHoverInteractInfos(InteractionContext context)
    {
        if (context == null || context.Player == null || context.Player.ItemHolder == null)
        {
            return AllowDispenseToPlayer
                ? EmptyHandInteractInfoCanTake
                : EmptyHandInteractInfoCantTake;
        }

        var holder = context.Player.ItemHolder;

        if (holder.isHolding &&
            holder.ItemInHand != null &&
            holder.ItemInHand.TryGetComponent(out HoldableContainer source) &&
            CanAccept(source))
        {
            return WhileHoldingContainerInfo;
        }

        return AllowDispenseToPlayer
            ? EmptyHandInteractInfoCanTake
            : EmptyHandInteractInfoCantTake;
    }

    public override HoverTooltipData GetHoverTooltipData()
    {
        return new HoverTooltipData(
            transform,
            () => Container != null ? Container.GetDisplayText() : "Empty"
        );
    }

    public override void Interact(InteractionContext context)
    {
        base.Interact(context);

        if (!isActiveAndEnabled)
            return;

        if (context.Type != DispenseToPlayerInteractType)
            return;

        if (!AllowDispenseToPlayer)
            return;

        var holder = context.Player?.ItemHolder;
        if (holder == null || Container == null)
            return;

        Container.TryDispenseToPlayer(holder);
    }
}