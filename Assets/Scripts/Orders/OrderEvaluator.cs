using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// result of evaluating a prepared item
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

public class OrderEvaluationResult
{
    public float Score;
    public int Discrepancies;


    public OrderEvaluationResult(float Score, int Discrepancies)
    {
        this.Score = Math.Clamp(Score, 0, 100);
        this.Discrepancies = Discrepancies;
    }

    public override string ToString()
    {
        return "Score: " + this.Score + "\n " + "Discrepancies: " + this.Discrepancies + "\n";

    }
}
/// <summary>
/// Matches against a snapshot to a menu item.
/// </summary>
public static class OrderEvaluator
{
    /// <summary>
    /// Evaluates an order and returns a reuslt
    /// 
    /// Given the list of Prepared Items, best matches the closes approximation. 
    ///     The closer it is- the higher the score.
    /// </summary>
    /// <param name="order"></param>
    /// <param name="preparedItems"></param>
    public static OrderEvaluationResult Evaluate(Order order, List<PreparedItemData> preparedItems)
    {
        int maxScore = order.MenuItems.Count;
        // match against the orders recipes.



        int totalDiscrepancies = 0;


        foreach (var preparedItem in preparedItems)
        {
            PreparedItemEvaluation eval = EvaluatePreparedItem(order.MenuItems, preparedItem);
            totalDiscrepancies += eval.Discrepancies;


        }

        float finalScore = 100;


        return new OrderEvaluationResult(finalScore, totalDiscrepancies);

    }

    static Dictionary<IngredientType, int> Consolidate(MenuItem item)
    {
        var map = new Dictionary<IngredientType, int>();

        foreach (var req in item.requirements)
        {
            if (!map.ContainsKey(req.Type))
                map[req.Type] = 0;

            map[req.Type] += req.Count;
        }

        return map;
    }
    public static PreparedItemEvaluation EvaluatePreparedItem(List<MenuItem> menuItems, PreparedItemData preparedItem)
    {
        UnityEngine.Debug.Log("Finding best match");
        // compare counts.
        Dictionary<MenuItem, int> discrepancyDict = new Dictionary<MenuItem, int>();

        foreach (var menuItem in menuItems)
        {

            Dictionary<IngredientType, int> consolidatedMenuItem = Consolidate(menuItem);
            int menuItemDiscrepancies = 0;
            // subtract the difference for each counts of ingredients between the snapshot and menu item.
            foreach (var (type, count) in consolidatedMenuItem)
            {
                if (preparedItem.Counts.TryGetValue(type, out var snapshotCount))
                {
                    int diff = Math.Abs(count - snapshotCount);
                    menuItemDiscrepancies += diff;
                }
                else
                {
                    menuItemDiscrepancies += count;
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