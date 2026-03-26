using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Represents an object that the player can interact with
/// </summary>
[RequireComponent(typeof(Collider))]
// [RequireComponent(typeof(Highlightable))]
public abstract class InteractableBase : MonoBehaviour, IInteractable
{

    public List<InteractInfo> HoverInteractInfo = new List<InteractInfo>();
    [Tooltip("Root object for whatever visually represents this item.")]
    public GameObject VisualRoot;
    // public string HoverTooltip = "";
    [HideInInspector]
    public HoverTooltipData HoverTooltipData;

    Outline outline = null;

    public event Action<InteractionContext> OnInteract;

    public bool IsHovered { get; private set; } = false;
    void Awake()
    {
        SetLayer();
    }
    void Start()
    {
        SetLayer();
        if (VisualRoot != null)
        {
            outline = VisualRoot.AddComponent<Outline>();
            outline.OutlineWidth = 5f;
            SetOutline(false);
        }
    }

    void OnValidate()
    {
        SetLayer();
    }
    public virtual void OnHoverEnter() { IsHovered = true; }
    public virtual void OnHoverExit() { IsHovered = false; }
    public void BaseInteract(InteractionContext context)
    {
        OnInteract?.Invoke(context);
        Interact(context);
    }


    public abstract void Interact(InteractionContext context);
    public virtual List<InteractInfo> GetHoverInteractInfos()
    {
        return HoverInteractInfo;
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


    public void SetOutline(bool boolean)
    {
        if (outline == null) return;

        outline.enabled = boolean;
    }

}