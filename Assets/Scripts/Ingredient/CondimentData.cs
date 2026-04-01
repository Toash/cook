using UnityEngine;

[CreateAssetMenu(menuName = "Ingredient/Condiment Data")]
public class CondimentData : HoldableData
{
    /// <summary>
    /// Prefab used when applying the condiment.
    /// </summary>
    public GameObject VisualPrefab;
}