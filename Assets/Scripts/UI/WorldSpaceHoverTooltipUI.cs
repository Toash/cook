
using TMPro;
using UnityEngine;

/// <summary>
/// Represents tooltip ui in world space
/// </summary>
public class WorldSpaceHoverTooltipUI : MonoBehaviour
{
    // needs to be child to make offsets work, as well as setting active and inactive.
    [Tooltip("What will be activated and deactivated, and offset. Should be a child of this object")]
    public GameObject Root;
    public TMP_Text TooltipText;
    public PlayerInteraction PlayerInteraction;


    void Awake()
    {
        if (Root == null)
        {
            Debug.LogError("[WorldSpaceHoverTooltipUI]: root not set!");
        }
        if (TooltipText == null)
        {
            Debug.LogError("[WorldSpaceHoverTooltipUI]: TooltipText not set!");
        }
        if (PlayerInteraction == null)
        {
            Debug.LogError("[WorldSpaceHoverTooltipUI]: PlayerInteraction not set!");
        }

    }
    void OnEnable()
    {
        PlayerInteraction.OnInteractableChanged += OnInteractableChanged;
        PlayerInteraction.OnInteractableInteracted += OnInteractableChanged;

    }
    void OnDisable()
    {
        PlayerInteraction.OnInteractableChanged -= OnInteractableChanged;
        PlayerInteraction.OnInteractableInteracted -= OnInteractableChanged;
    }

    void Update()
    {
        //billboard
        transform.LookAt(Camera.main.transform);

    }

    void OnInteractableChanged(InteractableBase interactable)
    {
        void Clear()
        {
            TooltipText.text = "";
            transform.SetParent(null, true);
            transform.position = Vector2.zero;
            Root.SetActive(false);
        }
        void Populate(HoverTooltipData data)
        {
            TooltipText.text = data.Text;
            transform.SetParent(data.Parent, true);
            transform.localPosition = Vector2.zero;
            Root.transform.localPosition = data.Offset;
            Root.SetActive(true);


        }
        if (interactable == null)
        {
            Clear();
        }
        else
        {
            var data = interactable.HoverTooltipData;
            if (data == null)
            {
                Clear();
            }
            else
            {
                Populate(data);
            }
        }


    }
}