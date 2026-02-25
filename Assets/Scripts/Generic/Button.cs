using UnityEngine.Events;

public class Button : InteractableBase
{

    public UnityEvent Pressed;


    public override void Interact(InteractionContext context)
    {
        UnityEngine.Debug.Log("Pressed button");
        Pressed.Invoke();
    }
}
