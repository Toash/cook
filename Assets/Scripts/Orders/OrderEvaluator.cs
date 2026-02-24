using System.Collections.Generic;

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
    /// <param name="submitted"></param>
    public static OrderEvaluationResult Evaluate(Order order, List<PreparedItemSnapshot> submitted)
    {
        int maxScore = order.MenuItems.Count;
        var res = new OrderEvaluationResult();
        // match against the orders recipes.




        return res;

    }

    /// <summary>
    /// Determines what is the most likely menu item for a snapshot.
    /// </summary>
    /// <param name="menuItems"></param>
    /// <param name="snapshot"></param>
    /// <returns></returns>
    // public static MenuItem FindBestMatch(List<MenuItem> menuItems, PreparedItemSnapshot snapshot)
    // {
    //     // compare counts.
    //     Dictionary<MenuItem, int> scores = new Dictionary<MenuItem, int>();

    //     foreach (var menuItem in menuItems)
    //     {
    //         // calculate counts of each ingredient between the menu item and the snapshot. 

    //         int score = snapshot.Counts

    //     }
    // }


}