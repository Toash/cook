using System.Collections.Generic;
using UnityEngine;
public class InteractionInfosUI : MonoBehaviour
{
    public PlayerInteraction PlayerInteraction;
    public InteractionInfoUI InfoPrefab;
    private List<InteractionInfoUI> activeInfos = new List<InteractionInfoUI>();


    public void OnEnable()
    {
        PlayerInteraction.OnInteractableChanged += OnInteractableChanged;
    }
    public void OnDisable()
    {
        PlayerInteraction.OnInteractableChanged -= OnInteractableChanged;
    }

    void OnInteractableChanged(IInteractable _)
    {
        var currentInteractable = PlayerInteraction.GetCurrentInteractable();
        if (currentInteractable != null)
        {
            ClearInfos();
            ShowInteractInfos(currentInteractable.GetInteractInfos());
        }
        else
        {
            ClearInfos();
        }


    }
    void ShowInteractInfos(List<InteractInfo> infos)
    {
        ClearInfos();

        foreach (var info in infos)
        {
            var infoGO = Instantiate(InfoPrefab, transform);
            infoGO.Populate(info);
            activeInfos.Add(infoGO);
        }
    }

    void ClearInfos()
    {
        foreach (var info in activeInfos)
        {
            Destroy(info.gameObject);
        }
        activeInfos.Clear();
    }
}