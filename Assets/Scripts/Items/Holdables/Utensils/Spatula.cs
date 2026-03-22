using System.Collections.Generic;
using UnityEngine;
public class Spatula : Holdable
{

    public List<InteractInfo> FlipInfos = new List<InteractInfo>();
    private Flippable currentFlippable = null;
    public override bool TryGetPlacementPreview(PlayerItemHolder holder, PlacementInfo info, out Vector3 pos, out Quaternion rot, out bool show)
    {
        // return base.TryGetPlacementPreviewPosAndRot(holder, info, out pos, out rot);
        if (currentFlippable)
        {
            pos = default;
            rot = default;
            show = false;
            return true;
        }
        pos = default;
        rot = default;
        show = false;
        return false;
    }
    public override bool OnPressedInteract(PlayerItemHolder holder, InteractionContext context)
    {
        if (context.Type == InteractType.Primary)
        {
            if (TryGetFlippable(context.Player.Interaction, out var flippable))
            {
                flippable.TryFlip();
                return true;

            }
        }
        return false;
    }
    public override List<InteractInfo> GetInteractInfos()
    {
        if (beingHeld)
        {
            if (currentFlippable != null)
            {
                return FlipInfos;
            }
            return HeldInfos;
        }
        else
        {
            return base.GetInteractInfos();
        }
    }

    protected override void OnAfterHeldInternal(PlayerItemHolder holder)
    {
        holder.Player.Interaction.OnHoveredInteractableChanged += OnHoveredInteractableChanged;
    }

    protected override void OnAfterPlaceInternal(PlayerItemHolder holder)
    {
        holder.Player.Interaction.OnHoveredInteractableChanged -= OnHoveredInteractableChanged;
    }

    void OnHoveredInteractableChanged(InteractableBase newInteractable)
    {
        if (newInteractable != null && newInteractable.TryGetComponent<Flippable>(out var flippable))
        {
            currentFlippable = flippable;
        }
        else
        {
            currentFlippable = null;
        }

    }
    bool TryGetFlippable(PlayerInteraction interaction, out Flippable flippable)
    {
        if (interaction.HoveredInteractable != null)
        {
            if (interaction.HoveredInteractable.TryGetComponent<Flippable>(out var outFlippable))
            {
                flippable = outFlippable;
                flippable.TryFlip();
                return true;

            }
        }
        flippable = null;
        return false;
    }
}