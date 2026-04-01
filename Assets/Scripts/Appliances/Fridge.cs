using UnityEngine;

/// <summary>
/// Something abuot "cooling" items
/// </summary>
public class Fridge : InteractableBase
{
    public Animator Animator;
    public string OpenParameter = "isOpen";


    public bool isOpen => Animator.GetBool(OpenParameter);

    public override void Interact(InteractionContext context)
    {
        Animator.SetBool(OpenParameter, !isOpen);
    }
}