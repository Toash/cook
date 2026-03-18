using System.Collections.Generic;
using UnityEngine;

public class HoldableSpawnDatabase : MonoBehaviour
{
    public static HoldableSpawnDatabase Instance { get; private set; }

    private readonly Dictionary<string, HoldableData> lookup = new();
    public HoldableData[] AllItems { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        AllItems = Resources.LoadAll<HoldableData>("ScriptableObjects/HoldableData");

        lookup.Clear();
        foreach (HoldableData item in AllItems)
        {
            if (item == null) continue;

            string key = item.Name.Trim().ToLower();
            if (!lookup.TryAdd(key, item))
            {
                Debug.LogWarning($"Duplicate HoldableData name found: {key}", item);
            }
        }
    }

    public HoldableData Get(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        lookup.TryGetValue(name.Trim().ToLower(), out var data);
        return data;
    }

    public HoldableData GetRandom()
    {
        if (AllItems == null || AllItems.Length == 0) return null;
        return AllItems[Random.Range(0, AllItems.Length)];
    }
}