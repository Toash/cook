using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pure storage logic for "N copies of one HoldableData".
/// No pickup or direct interaction behavior lives here.
/// </summary>
public class ItemContainer : MonoBehaviour
{
    [Header("Container")]
    public HoldableData ContainedItemData;
    public int Capacity = 1;
    public int CurrentAmount = 0;

    [Header("Optional Visuals")]
    [Tooltip("Enable first N objects based on CurrentAmount.")]
    public List<GameObject> StoredItemVisuals = new();

    private void Awake()
    {
        RefreshVisuals();
    }
    public void Init(HoldableData itemData, int currentAmount, int capacity)
    {
        ContainedItemData = itemData;
        Capacity = Mathf.Max(0, capacity);
        CurrentAmount = Mathf.Clamp(currentAmount, 0, Capacity);
        OnAmountChanged();
    }

    public bool IsEmpty()
    {
        return CurrentAmount <= 0;
    }

    public bool CanProvide(HoldableData itemData)
    {
        return ContainedItemData == itemData && CurrentAmount > 0;
    }

    public int RemoveUpTo(int amount)
    {
        if (amount <= 0 || CurrentAmount <= 0)
            return 0;

        int removed = Mathf.Min(amount, CurrentAmount);
        CurrentAmount -= removed;
        OnAmountChanged();
        return removed;
    }

    public int AddItems(int amount)
    {
        if (amount <= 0)
            return 0;

        int freeSpace = Mathf.Max(0, Capacity - CurrentAmount);
        int added = Mathf.Min(amount, freeSpace);

        if (added <= 0)
            return 0;

        CurrentAmount += added;
        OnAmountChanged();
        return added;
    }

    public bool TryRemoveOne()
    {
        return RemoveUpTo(1) > 0;
    }

    public bool TryDispenseToPlayer(PlayerItemHolder holder)
    {
        if (holder == null)
            return false;

        if (holder.isHolding)
            return false;

        if (CurrentAmount <= 0)
            return false;

        if (ContainedItemData == null || ContainedItemData.Prefab == null)
            return false;

        Holdable spawned = Instantiate(ContainedItemData.Prefab);

        if (!holder.TryHold(spawned))
        {
            Destroy(spawned.gameObject);
            return false;
        }

        CurrentAmount--;
        OnAmountChanged();
        return true;
    }

    public string GetDisplayText()
    {
        if (ContainedItemData == null)
            return "Empty";

        return $"{CurrentAmount} / {Capacity} {ContainedItemData.Name}s";
    }

    protected virtual void OnAmountChanged()
    {
        RefreshVisuals();
    }

    public void RefreshVisuals()
    {
        if (StoredItemVisuals == null || StoredItemVisuals.Count == 0)
            return;

        for (int i = 0; i < StoredItemVisuals.Count; i++)
        {
            if (StoredItemVisuals[i] != null)
            {
                StoredItemVisuals[i].SetActive(i < CurrentAmount);
            }
        }
    }
#if UNITY_EDITOR
    private void OnValidate()
    {
        Capacity = Mathf.Max(0, Capacity);
        CurrentAmount = Mathf.Clamp(CurrentAmount, 0, Capacity);
        RefreshVisuals();
    }
#endif
}