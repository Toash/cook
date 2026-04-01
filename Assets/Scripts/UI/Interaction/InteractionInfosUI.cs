using System.Collections.Generic;
using UnityEngine;
public class InteractionInfosUI : MonoBehaviour
{
    public PlayerInteraction PlayerInteraction;
    public PlayerItemHolder Holder;
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
        ClearInfos();

        if (Holder != null && Holder.isHolding)
        {
            var infos = Holder.ItemInHand.GetHeldInteractInfos(Holder);
            ShowInteractInfos(infos);
            return;
        }

        // fallback: hovered object
        var currentInteractable = PlayerInteraction.GetCurrentInteractable();
        if (currentInteractable != null)
        {
            ShowInteractInfos(currentInteractable.GetHoverInteractInfos(PlayerInteraction.BuildContext(InteractType.None)));
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