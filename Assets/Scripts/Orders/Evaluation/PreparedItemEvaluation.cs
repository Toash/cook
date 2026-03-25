using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Evaluation of a single prepared item
/// </summary>
public class PreparedItemAssemblyDiscrepancies
{
    public OrderedMenuItem ClosestMenuItem;

    public int Discrepancies;

    public PreparedItemAssemblyDiscrepancies(OrderedMenuItem closestMenuItem, int discrepancies)
    {
        ClosestMenuItem = closestMenuItem;
        Discrepancies = discrepancies;
    }

    public static PreparedItemAssemblyDiscrepancies EvaluateByMatchingClosestMenuItem(
        List<OrderedMenuItem> menuItems,
        PreparedItemData preparedItem)
    {
        Debug.Log("[PreparedItemAssemblyDiscrepancies]: Finding best match");

        if (menuItems == null || menuItems.Count == 0)
        {
            return new PreparedItemAssemblyDiscrepancies(null, 0);
        }

        OrderedMenuItem closestMenuItem = null;
        int lowestDiscrepancies = int.MaxValue;

        foreach (OrderedMenuItem orderedItem in menuItems)
        {
            if (orderedItem == null || orderedItem.BaseItem == null)
                continue;

            Dictionary<IngredientData, int> consolidatedMenuItem =
                BaseMenuItem.Consolidate(orderedItem.BaseItem);

            int discrepancies = 0;
            HashSet<IngredientData> allIngredients = new HashSet<IngredientData>();

            foreach (var kv in consolidatedMenuItem)
                allIngredients.Add(kv.Key);

            foreach (var kv in preparedItem.ConsolidatedCounts)
                allIngredients.Add(kv.Key);

            foreach (IngredientData ingredient in allIngredients)
            {
                int menuCount = consolidatedMenuItem.TryGetValue(ingredient, out var m) ? m : 0;
                int preparedCount = preparedItem.ConsolidatedCounts.TryGetValue(ingredient, out var p) ? p : 0;

                Debug.Log($"[PreparedItemAssemblyDiscrepancies]: Ingredient: {ingredient.Name}, Menu Count: {menuCount}, Prepared Count: {preparedCount}");
                discrepancies += Math.Abs(menuCount - preparedCount);
            }

            if (discrepancies < lowestDiscrepancies)
            {
                lowestDiscrepancies = discrepancies;
                closestMenuItem = orderedItem;
            }
        }

        if (closestMenuItem == null)
        {
            return new PreparedItemAssemblyDiscrepancies(null, 0);
        }

        return new PreparedItemAssemblyDiscrepancies(closestMenuItem, lowestDiscrepancies);
    }
}