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
    // public virtual string HoverTooltip { get; set; } = "";
    public string HoverTooltip = "";
    // List<Outline> outlines = new();

    Outline outline = null;
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
    public abstract void Interact(InteractionContext context);

}