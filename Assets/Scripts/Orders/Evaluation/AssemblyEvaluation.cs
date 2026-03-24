using System.Collections.Generic;
using System.Reflection;
using Assets.Scripts.Ingredient.MenuItem;
using UnityEngine;

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
        Debug.Log("[AssemblyEvaluation]: Evaluating assembly...");
        int expectedPartCount = 0;
        foreach (var menuItem in order.MenuItems)
        {
            foreach (var (_, count) in OrderMenuItem.Consolidate(menuItem))
            {
                expectedPartCount += count;
            }
        }

        if (expectedPartCount <= 0)
        {
            return new AssemblyEvaluation(0f, 0);
        }

        int totalDiscrepancies = 0;
        foreach (PreparedItemData preparedItem in preparedItems)
        {
            PreparedItemAssemblyDiscrepancies discrepancies =
                PreparedItemAssemblyDiscrepancies.EvaluateByMatchingClosestMenuItem(order.MenuItems, preparedItem);

            totalDiscrepancies += discrepancies.Discrepancies;
        }

        float assemblyScore = 1f - ((float)totalDiscrepancies / expectedPartCount);
        assemblyScore = Mathf.Clamp01(assemblyScore);
        Debug.Log("[AssemblyEvaluation]: Total discrepancies: " + totalDiscrepancies);
        Debug.Log("[AssemblyEvaluation]: Assembly score: " + assemblyScore);

        if (totalDiscrepancies > 1)
        {
            assemblyScore = 0f;
        }

        return new AssemblyEvaluation(assemblyScore, totalDiscrepancies);
    }
}