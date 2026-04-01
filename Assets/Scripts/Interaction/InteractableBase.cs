using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an object that the player can interact with.
/// Invariant: every InteractableBase root object must be on the "Interact" layer.
/// </summary>
[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    private const string INTERACT_LAYER_NAME = "Interact";

    public List<InteractInfo> HoverInteractInfo = new();

    [Tooltip("Root object for whatever visually represents this item.")]
    public GameObject VisualRoot;

    [HideInInspector]
    public HoverTooltipData HoverTooltipData;

    private Outline outline;

    public event Action<InteractionContext> OnInteract;

    public bool IsHovered { get; private set; }

    protected virtual void Reset()
    {
        EnforceInteractLayer();
    }

    protected virtual void Awake()
    {
        EnforceInteractLayer();
    }

    protected virtual void Start()
    {
        EnforceInteractLayer();

        if (VisualRoot == null)
        {
            Debug.LogError("[InteractableBase]: No VisualRoot assigned!");

        }


        if (VisualRoot != null)
        {
            outline = VisualRoot.GetComponent<Outline>();
            if (outline == null)
                outline = VisualRoot.AddComponent<Outline>();

            outline.OutlineWidth = 5f;
            SetOutline(false);
        }
    }

    protected virtual void OnValidate()
    {
        EnforceInteractLayer();
    }

    public virtual void OnHoverEnter()
    {
        IsHovered = true;
    }

    public virtual void OnHoverExit()
    {
        IsHovered = false;
    }

    public void BaseInteract(InteractionContext context)
    {
        OnInteract?.Invoke(context);
        Interact(context);
    }

    public abstract void Interact(InteractionContext context);

    public virtual List<InteractInfo> GetHoverInteractInfos(InteractionContext context)
    {
        return HoverInteractInfo;
    }

    // public void SetHoverTooltipData(HoverTooltipData data)
    // {
    //     HoverTooltipData = data;
    // }

    public virtual HoverTooltipData GetHoverTooltipData()
    {
        return HoverTooltipData;
    }

    public void SetOutline(bool enabled)
    {
        if (outline == null) return;
        outline.enabled = enabled;
    }

    private void EnforceInteractLayer()
    {
        int interactLayer = LayerMask.NameToLayer(INTERACT_LAYER_NAME);
        if (interactLayer == -1)
        {
            Debug.LogError($"Could not find required layer '{INTERACT_LAYER_NAME}'.", this);
            return;
        }

        if (gameObject.layer != interactLayer)
        {
            gameObject.layer = interactLayer;
        }
    }
}