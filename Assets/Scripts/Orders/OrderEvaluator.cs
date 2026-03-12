using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Ingredient.MenuItem;

/// <summary>
/// Evaluates a prepared item by matching it to the closest menu item, and counting discrepancies
/// </summary>
public class PreparedItemEvaluation
{
    public MenuItem ClosestMenuItem;
    public int Discrepancies;

    public PreparedItemEvaluation(MenuItem ClosestMenuItem, int Discrepancies)
    {
        this.ClosestMenuItem = ClosestMenuItem;
        this.Discrepancies = Discrepancies;
    }
}

/// <summary>
/// Evaluate an order to determine to what degree the PreparedItems matches the Menu Items of the order.
/// </summary>
public class OrderEvaluationResult
{
    public float Score01;
    public OrderScoreCategories ScoreCategory;
    public int Discrepancies;


    public OrderEvaluationResult(float Score, int Discrepancies)
    {
        this.Score01 = Math.Clamp(Score, 0, 1);
        this.Discrepancies = Discrepancies;
    }

    public override string ToString()
    {
        return "Score: " + this.Score01 + "\n " + "Discrepancies: " + this.Discrepancies + "\n";

    }
}
public enum OrderScoreCategories
{
    None,
    Bad,
    Okay,
    Good
}

/// <summary>
/// Matches against a snapshot to a menu item.
/// </summary>
public static class OrderEvaluator
{
    // starting cutoff points for categories
    public static Dictionary<OrderScoreCategories, float> ScoreCategories = new Dictionary<OrderScoreCategories, float>()
    {
        {OrderScoreCategories.Bad, 0},
        {OrderScoreCategories.Okay, .5f},
        {OrderScoreCategories.Good, .8f},


    };

    public static OrderScoreCategories GetScoreCategory(float score)
    {
        foreach (var (category, categoryScore) in ScoreCategories.Reverse())
        {
            if (score >= categoryScore)
            {
                return category;
            }

        }
        return OrderScoreCategories.None;

    }


    /// <summary>
    /// Evaluates an order and returns a reuslt
    /// 
    /// Given the list of Prepared Items, best matches the closes approximation. 
    ///     The closer it is- the higher the score. Does this for all of the prepared items.
    /// </summary>
    /// <param name="order"></param>
    /// <param name="preparedItems"></param>
    public static OrderEvaluationResult Evaluate(Order order, List<PreparedItemData> preparedItems)
    {
        int maxScore = 0;
        foreach (var menuItem in order.MenuItems)
        {
            foreach (var (data, count) in Consolidate(menuItem))
            {
                maxScore += count;
            }
        }
        // match against the orders recipes.



        int totalDiscrepancies = 0;


        foreach (var preparedItem in preparedItems)
        {
            PreparedItemEvaluation eval = PreparedItemMatchClosestMenuItem(order.MenuItems, preparedItem);
            totalDiscrepancies += eval.Discrepancies;


        }


        float normalizedScore = (float)totalDiscrepancies / maxScore;

        return new OrderEvaluationResult(normalizedScore, totalDiscrepancies);

    }

    /// <summary>
    /// Returns counts of ingredients in a menu item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    static Dictionary<IngredientData, int> Consolidate(MenuItem item)
    {
        var map = new Dictionary<IngredientData, int>();

        foreach (IngredientRequirement req in item.Requirements)
        {
            if (!map.ContainsKey(req.Data))
                map[req.Data] = 0;

            map[req.Data] += req.Count;
        }

        return map;
    }
    /// <summary>
    /// Evaluates prepared item by matchingg it to closest menu item
    /// </summary>
    /// <param name="menuItems"></param>
    /// <param name="preparedItem"></param>
    /// <returns></returns>
    public static PreparedItemEvaluation PreparedItemMatchClosestMenuItem(List<MenuItem> menuItems, PreparedItemData preparedItem)
    {
        UnityEngine.Debug.Log("Finding best match");
        // compare counts.
        Dictionary<MenuItem, int> discrepancyDict = new Dictionary<MenuItem, int>();

        foreach (MenuItem menuItem in menuItems)
        {

            Dictionary<IngredientData, int> consolidatedMenuItem = Consolidate(menuItem);
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

        return new PreparedItemEvaluation(closestMenuItem, preparedItemDiscrepancies);

    }


}