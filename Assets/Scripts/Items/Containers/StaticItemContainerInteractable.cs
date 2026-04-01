using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// World interactable for a non-portable item container.
/// </summary>
[RequireComponent(typeof(ItemContainer))]
public class StaticItemContainerInteractable : InteractableBase
{
    public ItemContainer Container;

    [Header("Dispense")]
    public InteractType DispenseToPlayerInteractType = InteractType.Secondary;
    public bool AllowDispenseToPlayer = true;

    [Header("UI")]
    public List<InteractInfo> CanTakeInfo = new()
    {
        new InteractInfo(InteractType.Secondary, "Take item")
    };

    public List<InteractInfo> CantTakeInfo = new();

    protected override void OnValidate()
    {
        base.OnValidate();

        if (Container == null)
            Container = GetComponent<ItemContainer>();
    }

    protected override void Awake()
    {
        base.Awake();
        if (Container == null)
            Container = GetComponent<ItemContainer>();
    }

    public override List<InteractInfo> GetHoverInteractInfos(InteractionContext context)
    {
        return AllowDispenseToPlayer ? CanTakeInfo : CantTakeInfo;
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

        if (!isActiveAndEnabled || Container == null)
            return;

        if (context.Type != DispenseToPlayerInteractType)
            return;

        var holder = context.Player?.ItemHolder;
        if (holder == null || holder.isHolding)
            return;

        if (AllowDispenseToPlayer)
        {
            Container.TryDispenseToPlayer(holder);
        }
    }
}