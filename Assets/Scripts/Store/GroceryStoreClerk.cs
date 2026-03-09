using UnityEngine;

/// <summary>
/// Clerk to interface with grocery store.
/// </summary>
public class GroceryStoreClerk : InteractableBase
{


    public override void Interact(InteractionContext context)
    {
        // open store ui
        context.Player.Controller.ShowPopup(PopupType.GroceryStore);
    }
}