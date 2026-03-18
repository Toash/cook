using UnityEngine;

[CreateAssetMenu(menuName = "Console Commands/List Holdables")]
public class ListHoldablesCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        var db = HoldableSpawnDatabase.Instance;

        if (db == null)
        {
            Debug.LogWarning("No HoldableSpawnDatabase found in scene.");
            return false;
        }

        var items = db.AllItems;

        if (items == null || items.Length == 0)
        {
            Debug.Log("No HoldableData found.");
            return true;
        }

        Debug.Log("=== Holdables ===");

        int count = 0;

        foreach (var item in items)
        {
            if (item == null) continue;

            string key = item.Name.Trim().ToLower();

            Debug.Log($"- {key}");
            count++;
        }

        Debug.Log($"Total: {count}");

        return true;
    }
}