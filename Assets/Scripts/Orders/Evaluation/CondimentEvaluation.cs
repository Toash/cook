using System.Collections.Generic;

public class CondimentEvaluation
{
    public int TotalExpectedCondiments;
    public int Discrepancies;
    public float Score01;

    public CondimentEvaluation(int totalExpectedCondiments, int discrepancies)
    {
        TotalExpectedCondiments = totalExpectedCondiments;
        Discrepancies = discrepancies;

        Score01 = discrepancies > 0 ? 0f : 1f;
    }

    public static CondimentEvaluation Evaluate(Order order, List<PreparedItemData> preparedItems)
    {
        if (order == null || order.Items == null || preparedItems == null)
            return new CondimentEvaluation(0, 0);

        int totalExpectedCondiments = 0;
        int totalDiscrepancies = 0;

        foreach (OrderedMenuItem orderedItem in order.Items)
        {
            if (orderedItem == null || orderedItem.AddedCondiments == null)
                continue;

            totalExpectedCondiments += orderedItem.AddedCondiments.Count;
        }

        foreach (PreparedItemData preparedItem in preparedItems)
        {
            PreparedItemAssemblyDiscrepancies closestMatch =
                PreparedItemAssemblyDiscrepancies.EvaluateByMatchingClosestMenuItem(order.Items, preparedItem);

            if (closestMatch == null || closestMatch.ClosestMenuItem == null)
                continue;

            CondimentEvaluation singleEval = EvaluateSingle(closestMatch.ClosestMenuItem, preparedItem);
            totalDiscrepancies += singleEval.Discrepancies;
        }

        return new CondimentEvaluation(totalExpectedCondiments, totalDiscrepancies);
    }

    public static CondimentEvaluation EvaluateSingle(OrderedMenuItem orderedItem, PreparedItemData preparedItem)
    {
        if (orderedItem == null || preparedItem == null)
            return new CondimentEvaluation(0, 0);

        var expected = new List<CondimentData>(orderedItem.AddedCondiments);
        var actual = GetActualCondiments(preparedItem);

        int discrepancies = 0;

        foreach (var expectedCondiment in expected)
        {
            if (!actual.Remove(expectedCondiment))
            {
                discrepancies++;
            }
        }

        discrepancies += actual.Count;

        return new CondimentEvaluation(expected.Count, discrepancies);
    }

    static List<CondimentData> GetActualCondiments(PreparedItemData preparedItem)
    {
        var result = new List<CondimentData>();

        if (preparedItem.RuntimeIngredients == null)
            return result;

        foreach (var ingredient in preparedItem.RuntimeIngredients)
        {
            if (ingredient == null) continue;

            if (ingredient.TryGetComponent<CondimentReceiver>(out var receiver))
            {
                foreach (var condiment in receiver.AppliedCondiments)
                {
                    if (condiment != null)
                        result.Add(condiment);
                }
            }
        }

        return result;
    }
}