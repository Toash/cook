
using UnityEngine;

/// <summary>
/// Represents an object that the player can interact with
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Highlightable))]
public abstract class InteractableBase : MonoBehaviour
{

    // public virtual string HoverTooltip { get; set; } = "";
    public string HoverTooltip = "";
    void Awake()
    {
        SetLayer();
    }
    void Start()
    {
        SetLayer();

    }
    void OnValidate()
    {
        SetLayer();
    }

    void SetLayer()
    {
        int interactLayer = LayerMask.NameToLayer("Interact");
        if (interactLayer == -1)
        {
            Debug.LogError("Could not find interact mask.");
            return;
        }

        gameObject.layer = interactLayer;

    }

    public virtual void OnHoverEnter() { }
    public virtual void OnHoverExit() { }
    public abstract void Interact(InteractionContext context);

}