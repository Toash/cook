
using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts.Ingredient.MenuItem;

/// <summary>
/// Evaluation of a single prepared item
/// </summary>
public class PreparedItemAssemblyDiscrepancies
{
    public OrderMenuItem ClosestMenuItem;

    public int Discrepancies;

    public PreparedItemAssemblyDiscrepancies(OrderMenuItem ClosestMenuItem, int Discrepancies)
    {
        this.ClosestMenuItem = ClosestMenuItem;
        this.Discrepancies = Discrepancies;
    }


    public static PreparedItemAssemblyDiscrepancies EvaluateByMatchingClosestMenuItem(
    List<OrderMenuItem> menuItems,
    PreparedItemData preparedItem)
    {
        Debug.Log("[PreparedItemAssemblyDiscrepancies]: Finding best match");

        if (menuItems == null || menuItems.Count == 0)
        {
            return new PreparedItemAssemblyDiscrepancies(null, 0);
        }

        OrderMenuItem closestMenuItem = null;
        int lowestDiscrepancies = int.MaxValue;

        foreach (OrderMenuItem menuItem in menuItems)
        {
            Dictionary<IngredientData, int> consolidatedMenuItem = OrderMenuItem.Consolidate(menuItem);
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

            // menu item with lowest discrepancies 
            if (discrepancies < lowestDiscrepancies)
            {
                lowestDiscrepancies = discrepancies;
                closestMenuItem = menuItem;
            }
        }

        return new PreparedItemAssemblyDiscrepancies(closestMenuItem, lowestDiscrepancies);
    }
}
