using UnityEngine;

/// <summary>
/// Clerk to interface with grocery store.
/// </summary>
public class GroceryStoreClerk : InteractableBase
{


    void Start()
    {
        HoverTooltipData = new HoverTooltipData(transform, "Shop");
    }

    public override void Interact(InteractionContext context)
    {
        // open store ui
        context.Player.Controller.ShowPopup(PopupType.GroceryStore);
    }
}