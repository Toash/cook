using System;
using System.Collections.Generic;

[Serializable]
public class SupplyEntry
{
    public HoldableData Item;
    public int Amount;
}

/// <summary>
///  Contains a list of items and their respective amounts.
/// </summary>
public class SupplyOrder
{
    public List<SupplyEntry> Items = new();
}