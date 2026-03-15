
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Ingredient.MenuItem;

/// <summary>
/// Evaluation of a single prepared item
/// </summary>
public class PreparedItemAssemblyDiscrepancies
{
    public MenuItem ClosestMenuItem;

    public int Discrepancies;

    public PreparedItemAssemblyDiscrepancies(MenuItem ClosestMenuItem, int Discrepancies)
    {
        this.ClosestMenuItem = ClosestMenuItem;
        this.Discrepancies = Discrepancies;
    }
    /// <summary>
    /// Evaluates prepared item by matching it to closest menu item
    /// </summary>
    /// <param name="menuItems"></param>
    /// <param name="preparedItem"></param>
    /// <returns></returns>
    public static PreparedItemAssemblyDiscrepancies Evaluate(List<MenuItem> menuItems, PreparedItemData preparedItem)
    {
        UnityEngine.Debug.Log("Finding best match");
        // compare counts.
        Dictionary<MenuItem, int> discrepancyDict = new Dictionary<MenuItem, int>();

        foreach (MenuItem menuItem in menuItems)
        {

            Dictionary<IngredientData, int> consolidatedMenuItem = MenuItem.Consolidate(menuItem);
            int menuItemDiscrepancies = 0;
            // subtract the difference for each counts of ingredients between the snapshot and menu item.
            foreach (var (ingredientData, menuItemIngredientCount) in consolidatedMenuItem)
            {
                if (preparedItem.Counts.TryGetValue(ingredientData, out var ingredientCount))
                {
                    int diff = Math.Abs(menuItemIngredientCount - ingredientCount);
                    menuItemDiscrepancies += diff;
                }
                else
                {
                    menuItemDiscrepancies += menuItemIngredientCount;
                }

            }



            // update score for that menu item 
            discrepancyDict.Add(menuItem, menuItemDiscrepancies);

        }
        UnityEngine.Debug.Log("Scores dictionary for finding best match: " + discrepancyDict.ToString());

        // return menu item with the highest score.
        MenuItem closestMenuItem = discrepancyDict.OrderBy(kv => kv.Value).First().Key;
        int preparedItemDiscrepancies = discrepancyDict[closestMenuItem];

        return new PreparedItemAssemblyDiscrepancies(closestMenuItem, preparedItemDiscrepancies);

    }
}
