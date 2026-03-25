using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Evaluates based off of how ingredients are actually laid out.
/// Only evaluates the base menu item assembly.
/// </summary>
public class AssemblyEvaluation
{
    public float Score01;
    public int Discrepancies;

    public AssemblyEvaluation(float score01, int discrepancies)
    {
        Score01 = score01;
        Discrepancies = discrepancies;
    }

    public static AssemblyEvaluation Evaluate(Order order, List<PreparedItemData> preparedItems)
    {
        Debug.Log("[AssemblyEvaluation]: Evaluating assembly...");

        int expectedPartCount = 0;

        foreach (OrderedMenuItem orderedItem in order.Items)
        {
            if (orderedItem == null || orderedItem.BaseItem == null)
                continue;

            foreach (var (_, count) in BaseMenuItem.Consolidate(orderedItem.BaseItem))
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
                PreparedItemAssemblyDiscrepancies.EvaluateByMatchingClosestMenuItem(order.Items, preparedItem);

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