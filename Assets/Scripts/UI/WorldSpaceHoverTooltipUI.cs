using TMPro;
using UnityEngine;

/// <summary>
/// Represents tooltip ui in world space
/// </summary>
public class WorldSpaceHoverTooltipUI : MonoBehaviour
{
    [Tooltip("What will be activated and deactivated, and offset. Should be a child of this object")]
    public GameObject Root;
    public TMP_Text TooltipText;
    public PlayerInteraction PlayerInteraction;

    public HoverTooltipData CurrentHoverData { get; private set; }
    public Transform CurrentTarget { get; private set; }

    void Awake()
    {
        if (Root == null)
            Debug.LogError("[WorldSpaceHoverTooltipUI]: root not set!");

        if (TooltipText == null)
            Debug.LogError("[WorldSpaceHoverTooltipUI]: TooltipText not set!");

        if (PlayerInteraction == null)
            Debug.LogError("[WorldSpaceHoverTooltipUI]: PlayerInteraction not set!");
    }

    void OnEnable()
    {
        PlayerInteraction.OnHoveredInteractableChanged += OnInteractableChanged;
        PlayerInteraction.OnHoveredInteractableInteracted += OnInteractableChanged;
    }

    void OnDisable()
    {
        PlayerInteraction.OnHoveredInteractableChanged -= OnInteractableChanged;
        PlayerInteraction.OnHoveredInteractableInteracted -= OnInteractableChanged;
    }

    void Update()
    {
        if (CurrentHoverData != null && CurrentTarget != null)
        {
            // transform.position = CurrentTarget.position + CurrentHoverData.Offset;
            transform.position = CurrentTarget.position;
            transform.LookAt(Camera.main.transform);
        }
        else if (CurrentHoverData != null && CurrentTarget == null)
        {
            Clear();
        }
    }

    void LateUpdate()
    {
        if (CurrentHoverData != null)
        {
            TooltipText.text = CurrentHoverData.GetText;
        }
    }

    void OnInteractableChanged(InteractableBase interactable)
    {
        if (interactable == null)
        {
            Clear();
            return;
        }

        var data = interactable.HoverTooltipData;
        if (data == null || data.Parent == null)
        {
            Clear();
            return;
        }

        Populate(data);
    }

    void Clear()
    {
        TooltipText.text = "";
        CurrentHoverData = null;
        CurrentTarget = null;
        Root.SetActive(false);
    }

    void Populate(HoverTooltipData data)
    {
        CurrentHoverData = data;
        CurrentTarget = data.Parent;

        TooltipText.text = data.GetText;
        transform.position = CurrentTarget.position + data.Offset;
        transform.LookAt(Camera.main.transform);

        Root.SetActive(true);
    }
}