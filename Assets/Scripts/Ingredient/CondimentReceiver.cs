using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Ingredient))]
public class CondimentReceiver : MonoBehaviour
{
    [Tooltip("The transform where condiment visuals will be spawned.")]
    public Transform CondimentVisualRoot;
    [ReadOnly]
    public Ingredient Ingredient;

    [ShowInInspector, ReadOnly]
    private readonly List<CondimentData> appliedCondiments = new();
    public IReadOnlyList<CondimentData> AppliedCondiments => appliedCondiments;

    void OnValidate()
    {
        if (Ingredient == null)
            Ingredient = GetComponent<Ingredient>();
    }
    void Awake()
    {
        if (Ingredient == null)
            Ingredient = GetComponent<Ingredient>();
    }

    public bool HasCondiment(CondimentData condiment)
    {
        return condiment != null && appliedCondiments.Contains(condiment);
    }

    public bool CanApplyCondiment(CondimentData condiment)
    {
        if (condiment == null) return false;
        if (Ingredient == null || Ingredient.Data == null) return false;
        if (HasCondiment(condiment)) return false;

        return true;
    }

    public bool TryApplyCondiment(CondimentData condiment)
    {
        Debug.Log($"[CondimentReceiver]: Trying to apply condiment {condiment?.Name} to ingredient {Ingredient?.Data?.Name}.");
        if (!CanApplyCondiment(condiment))
            return false;

        appliedCondiments.Add(condiment);
        SpawnVisual(condiment);

        Debug.Log("[CondimentReceiver]: Condiment applied successfully.");
        return true;
    }

    void SpawnVisual(CondimentData condiment)
    {
        if (CondimentVisualRoot == null || condiment.VisualPrefab == null)
            return;

        var instance = Instantiate(condiment.VisualPrefab, CondimentVisualRoot);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;
    }
}