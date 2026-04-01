using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A portable container that holds multiple copies of one item type.
/// Can:
/// - be picked up
/// - dispense one item to the player while in the world
/// - transfer items into compatible BulkSourceContainers when held
/// </summary>
[RequireComponent(typeof(ItemContainer))]
public class HoldableContainer : Holdable
{
    [Header("Container")]
    public ItemContainer Container;

    [Header("Dispense")]
    public InteractType DispenseToPlayerInteractType = InteractType.Secondary;
    public bool AllowDispenseToPlayer = true;

    [Header("Transfer")]
    public bool AllowTransferToBulkSourceContainer = true;
    public bool DestroyWhenEmpty = true;

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

    [Tooltip("Shown when hovering over a compatible BulkSourceContainer while this is held.")]
    public List<InteractInfo> HoverContainerInfo = new()
    {
        new InteractInfo(InteractType.Primary, "Load")
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

    public void Init(HoldableData itemData, int startingAmount)
    {
        if (Container == null)
            Container = GetComponent<ItemContainer>();

        Container.Init(itemData, startingAmount, startingAmount);
    }

    public bool IsEmpty()
    {
        return Container == null || Container.IsEmpty();
    }

    public bool CanProvide(HoldableData itemData)
    {
        return Container != null && Container.CanProvide(itemData);
    }

    public int RemoveUpTo(int amount)
    {
        if (Container == null)
            return 0;

        int removed = Container.RemoveUpTo(amount);

        if (DestroyWhenEmpty && Container.IsEmpty())
        {
            Destroy(gameObject);
        }

        return removed;
    }

    public bool CanTransferTo(BulkSourceContainer bulk)
    {
        if (!AllowTransferToBulkSourceContainer)
            return false;

        if (bulk == null || bulk.Container == null || Container == null)
            return false;

        if (Container.ContainedItemData == null)
            return false;

        return bulk.CanAccept(this);
    }

    public override List<InteractInfo> GetHoverInteractInfos(InteractionContext context)
    {
        if (AllowDispenseToPlayer)
            return EmptyHandInteractInfoCanTake;

        return EmptyHandInteractInfoCantTake;
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

        if (DestroyWhenEmpty && Container.IsEmpty())
        {
            Destroy(gameObject);
        }
    }

    public override List<InteractInfo> GetHeldInteractInfos(PlayerItemHolder holder)
    {
        if (holder == null || holder.Player?.Interaction == null)
            return base.GetHeldInteractInfos(holder);

        var hovered = holder.Player.Interaction.HoveredInteractable;

        if (hovered != null &&
            hovered.TryGetComponent(out BulkSourceContainer bulk) &&
            CanTransferTo(bulk))
        {
            return HoverContainerInfo;
        }

        return base.GetHeldInteractInfos(holder);
    }

    /// <summary>
    /// if hovering over a bulk source container, try to add to it
    /// </summary>
    /// <param name="holder"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override bool OnHeldPressedInteract(PlayerItemHolder holder, InteractionContext context)
    {
        if (context == null || context.Type != InteractType.Primary)
            return false;

        var hovered = context.Player?.Interaction?.HoveredInteractable;
        if (hovered == null)
            return false;

        if (!hovered.TryGetComponent(out BulkSourceContainer bulk))
            return false;

        if (!CanTransferTo(bulk))
            return true;

        int freeSpace = bulk.Container.Capacity - bulk.Container.CurrentAmount;
        int removed = RemoveUpTo(freeSpace);

        if (removed <= 0)
            return true;

        bulk.Container.AddItems(removed);

        if (DestroyWhenEmpty && IsEmpty())
        {
            holder.TryDeleteHeldItem();
        }

        return true;
    }

    public override bool TryGetCustomPlacementPreview(
        PlayerItemHolder holder,
        PlacementInfo info,
        out Vector3 pos,
        out Quaternion rot,
        out bool show)
    {
        if (holder != null &&
            holder.Player != null &&
            holder.Player.Interaction != null)
        {
            var hovered = holder.Player.Interaction.HoveredInteractable;

            if (hovered != null &&
                hovered.TryGetComponent(out BulkSourceContainer bulk) &&
                CanTransferTo(bulk))
            {
                pos = default;
                rot = default;
                show = false;
                return true;
            }
        }

        pos = default;
        rot = default;
        show = false;
        return false;
    }
}