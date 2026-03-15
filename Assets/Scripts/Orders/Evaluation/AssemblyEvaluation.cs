using System.Collections.Generic;
using System.Reflection;
using Assets.Scripts.Ingredient.MenuItem;

/// <summary>
/// Evaluates based off of how ingredients are actually laid out.
/// </summary>
public class AssemblyEvaluation
{
    public float Score01;
    public int Discrepancies;
    public AssemblyEvaluation(float score01, int discrepancies)
    {
        this.Score01 = score01;
        this.Discrepancies = discrepancies;
    }

    public static AssemblyEvaluation Evaluate(Order order, List<PreparedItemData> preparedItems)
    {
        int maxScore = 0;
        foreach (var menuItem in order.MenuItems)
        {
            foreach (var (data, count) in MenuItem.Consolidate(menuItem))
            {
                maxScore += count;
            }
        }



        int totalDiscrepancies = 0;


        foreach (var preparedItem in preparedItems)
        {
            PreparedItemAssemblyDiscrepancies assemblyDiscrepancies = PreparedItemAssemblyDiscrepancies.Evaluate(order.MenuItems, preparedItem);
            totalDiscrepancies += assemblyDiscrepancies.Discrepancies;


        }


        float assemblyScore = (float)totalDiscrepancies / maxScore;


        if (totalDiscrepancies > 1)
        {
            assemblyScore = 0;
        }

        return new AssemblyEvaluation(assemblyScore, totalDiscrepancies);

    }
}