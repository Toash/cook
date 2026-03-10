using System;
using UnityEngine;
/// <summary>
/// Represents an object that the player can interact with
/// </summary>
[RequireComponent(typeof(Collider))]
// [RequireComponent(typeof(Highlightable))]
public abstract class InteractableBase : MonoBehaviour
{

    [Tooltip("Root object for whatever visually represents this item.")]
    public GameObject VisualRoot;
    // public string HoverTooltip = "";
    [HideInInspector]
    public HoverTooltipData HoverTooltipData;

    Outline outline = null;

    public event Action<InteractionContext> OnInteract;
    void Awake()
    {
        SetLayer();
    }
    void Start()
    {
        SetLayer();
        if (VisualRoot != null)
        {
            // foreach (var obj in VisualRoot.GetComponentsInChildren<Transform>())
            // {
            //     if (obj == VisualRoot.transform) continue;
            //     var outline = obj.AddComponent<Outline>();
            //     outline.OutlineWidth = 8f;
            //     outlines.Add(outline);
            // }
            outline = VisualRoot.AddComponent<Outline>();
            outline.OutlineWidth = 5f;
            SetOutline(false);


        }
    }
    public void SetOutline(bool boolean)
    {
        if (outline == null) return;

        outline.enabled = boolean;
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
    public void BaseInteract(InteractionContext context)
    {
        OnInteract?.Invoke(context);
        Interact(context);
    }


    public abstract void Interact(InteractionContext context);


}