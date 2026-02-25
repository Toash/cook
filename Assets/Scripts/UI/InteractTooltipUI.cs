
using TMPro;
using UnityEngine;

public class InteractTooltipUI : MonoBehaviour
{
    public TMP_Text TooltipText;
    public PlayerInteraction PlayerInteraction;


    void OnEnable()
    {
        PlayerInteraction.OnInteractableChanged += OnInteractableChanged;
    }
    void OnDisable()
    {
        PlayerInteraction.OnInteractableChanged -= OnInteractableChanged;
    }

    void OnInteractableChanged(InteractableBase interactable)
    {
        if (interactable == null)
        {
            TooltipText.text = "";
        }
        else
        {
            TooltipText.text = interactable.HoverTooltip;
        }


    }
}