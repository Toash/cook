
using UnityEngine;

public interface IInteractable
{
    public Transform GetTransform();
    public void Interact(InteractionContext context);
    // public void SecondaryInteract(InteractionContext context);

}