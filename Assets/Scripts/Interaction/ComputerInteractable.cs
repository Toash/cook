using System.Collections.Generic;
using UnityEngine;

public class ComputerInteractable : InteractableBase
{
    [Header("Popup")]
    public PopupType PopupType = PopupType.Computer;
    public bool AllowMovementWhileOpen = false;



    public override void Interact(InteractionContext context)
    {
        if (context == null || context.Player == null)
        {
            Debug.LogWarning("[ComputerInteractable]: Missing interaction context or player.");
            return;
        }

        PlayerController controller = context.Player.Controller;
        if (controller == null)
        {
            Debug.LogWarning("[ComputerInteractable]: PlayerController missing on player.");
            return;
        }

        controller.ShowPopup(PopupType, AllowMovementWhileOpen);
    }

}