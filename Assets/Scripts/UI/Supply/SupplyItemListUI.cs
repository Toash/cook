using System.Collections.Generic;
using UnityEngine;

public class SupplyItemListUI : MonoBehaviour
{
    [Header("Data")]
    public List<HoldableData> Items;

    [Header("Refs")]
    public SupplyItemUI ItemPrefab;
    public Transform Container;
    public SupplyOrderController Controller;

    private readonly List<SupplyItemUI> spawnedItems = new();

    void Start()
    {
        Build();
    }

    public void Build()
    {
        Clear();

        if (ItemPrefab == null || Container == null)
        {
            Debug.LogWarning("[SupplyItemListUI]: Missing prefab or container.");
            return;
        }

        foreach (HoldableData item in Items)
        {
            if (item == null) continue;

            SupplyItemUI ui = Instantiate(ItemPrefab, Container);
            ui.Item = item;
            ui.Controller = Controller;

            spawnedItems.Add(ui);
        }
    }

    public void Clear()
    {
        foreach (var ui in spawnedItems)
        {
            if (ui != null)
            {
                Destroy(ui.gameObject);
            }
        }

        spawnedItems.Clear();
    }
}