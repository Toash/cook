using System.Collections.Generic;

/// <summary>
/// Evaluates based off of how well ingredients are cooked.
/// Discrepancies = number of ingredients that are not in the Cooked state.
/// </summary>
public class CookingEvaluation
{
    public int TotalIngredients;
    public int Discrepancies;
    public float Score01;

    public CookingEvaluation(int totalIngredients, int discrepancies)
    {
        TotalIngredients = totalIngredients;
        Discrepancies = discrepancies;

        if (discrepancies > 0)
        {
            Score01 = 0f;
        }
        else
        {
            Score01 = 1f;
        }
    }

    /// <summary>
    /// Evaluate the ingredients within the prepared items for cook state.
    /// </summary>
    /// <param name="order"></param>
    /// <param name="preparedItems"></param>
    /// <returns></returns>
    public static CookingEvaluation Evaluate(Order order, List<PreparedItemData> preparedItems)
    {
        if (preparedItems == null || preparedItems.Count == 0)
        {
            return new CookingEvaluation(0, 0);
        }

        int totalIngredients = 0;
        int discrepancies = 0;

        foreach (PreparedItemData preparedItem in preparedItems)
        {
            if (preparedItem == null || preparedItem.RuntimeIngredients == null)
                continue;

            foreach (Ingredient ingredient in preparedItem.RuntimeIngredients)
            {
                if (ingredient == null)
                    continue;

                totalIngredients++;

                if (ingredient.TryGetCookable(out var cookable))
                {
                    if (cookable.CookState != CookState.Cooked)
                    {
                        discrepancies++;
                    }

                }
            }
        }

        return new CookingEvaluation(totalIngredients, discrepancies);
    }
}
