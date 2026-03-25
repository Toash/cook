using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/Condiment Database")]
public class CondimentDatabase : ScriptableObject
{
    public List<CondimentData> AllCondiments;

    public CondimentData GetRandom()
    {
        if (AllCondiments == null || AllCondiments.Count == 0) return null;
        return AllCondiments[Random.Range(0, AllCondiments.Count)];
    }
}