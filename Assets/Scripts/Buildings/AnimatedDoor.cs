using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Changes bool parameter on a Animator, when interacting.
/// </summary>
public class AnimatedDoor : InteractableBase
{
    public UnityEvent OnOpen;
    public UnityEvent OnClose;

    public Animator Animator;
    public string OpenParameterName = "isOpen";


    public override void Interact(InteractionContext context)
    {
        // play animator
        Animator.SetBool(OpenParameterName, !Animator.GetBool(OpenParameterName));

        if (Animator.GetBool(OpenParameterName) == true)
        {
            OnOpen.Invoke();
        }
        else
        {
            OnClose.Invoke();

        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.Label(transform.position, "AnimatedDoor");
    }
#endif
}