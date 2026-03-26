using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SupplyEntry
{
    public HoldableData Item;
    public int Amount;
}

/// <summary>
/// Contains a list of items and their respective amounts.
/// </summary>
[Serializable]
public class SupplyOrder
{
    public List<SupplyEntry> Items = new();

    public SupplyEntry GetEntry(HoldableData item)
    {
        if (item == null) return null;
        return Items.Find(x => x.Item == item);
    }

    public int GetAmount(HoldableData item)
    {
        var entry = GetEntry(item);
        return entry != null ? entry.Amount : 0;
    }

    public void SetAmount(HoldableData item, int amount)
    {
        if (item == null) return;

        var entry = GetEntry(item);

        if (amount <= 0)
        {
            if (entry != null)
            {
                Items.Remove(entry);
            }
            return;
        }

        if (entry == null)
        {
            Items.Add(new SupplyEntry
            {
                Item = item,
                Amount = amount
            });
        }
        else
        {
            entry.Amount = amount;
        }
    }

    public void AddAmount(HoldableData item, int amount)
    {
        if (item == null || amount <= 0) return;
        SetAmount(item, GetAmount(item) + amount);
    }

    public void RemoveAmount(HoldableData item, int amount)
    {
        if (item == null || amount <= 0) return;
        SetAmount(item, GetAmount(item) - amount);
    }

    public void Clear()
    {
        Items.Clear();
    }

    public bool IsEmpty()
    {
        return Items.Count == 0;
    }
}