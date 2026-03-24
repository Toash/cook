using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Represents an actual ingredient in the world
/// </summary>
[RequireComponent(typeof(Snapper))]
[RequireComponent(typeof(Holdable))]
public class Ingredient : MonoBehaviour
{
    public IngredientData Data;

    [SerializeField] private List<CondimentData> appliedCondiments = new();
    public IReadOnlyList<CondimentData> AppliedCondiments => appliedCondiments;

    // what food this ingredient belongs to, if any.
    public PreparedItem PreparedItem { get; set; }
    public Snapper Snapper { get; private set; }

    void InitRef()
    {
        if (TryGetComponent<Collider>(out var collider))
        {
            collider.isTrigger = true;
        }

        if (Snapper == null)
        {
            Snapper = GetComponent<Snapper>();
            Snapper.SetSnapType(SnapType.Food);
        }
    }

    void OnValidate()
    {
        InitRef();
    }

    void Awake()
    {
        InitRef();
    }

    void Start()
    {
        if (TryGetComponent<InteractableBase>(out var interactable))
        {
            interactable.HoverTooltipData = new HoverTooltipData(transform, GetTooltipText);
        }
    }

    string GetTooltipText()
    {
        string baseName = Data != null ? Data.Name : "Missing Ingredient Data";

        if (TryGetComponent<Cookable>(out var cookable))
        {
            baseName = $"{cookable.CookState} {baseName}";
        }

        if (appliedCondiments.Count == 0)
            return baseName;

        return $"{baseName} [{string.Join(", ", GetCondimentNames())}]";
    }

    IEnumerable<string> GetCondimentNames()
    {
        foreach (CondimentData condiment in appliedCondiments)
        {
            if (condiment != null)
                yield return condiment.Name;
        }
    }

    public bool HasCondiment(CondimentData condiment)
    {
        return condiment != null && appliedCondiments.Contains(condiment);
    }

    public bool CanApplyCondiment(CondimentData condiment)
    {
        if (condiment == null) return false;

        if (HasCondiment(condiment))
            return false;

        return true;
    }
    public bool TryApplyCondiment(CondimentData condiment)
    {
        if (!CanApplyCondiment(condiment))
            return false;

        appliedCondiments.Add(condiment);

        return true;
    }

    public bool RemoveCondiment(CondimentData condiment)
    {
        if (condiment == null)
            return false;

        return appliedCondiments.Remove(condiment);
    }

    public Cookable TryGetCookable(out Cookable cookable)
    {
        return TryGetComponent<Cookable>(out cookable) ? cookable : null;
    }

    public override string ToString()
    {
        return Data != null ? Data.Name : base.ToString();
    }

#if UNITY_EDITOR
    GUIStyle style = new();

    void OnDrawGizmosSelected()
    {
        style.normal.textColor = Color.green;

        if (Data != null)
        {
            string label = $"Ingredient: {Data.Name}";

            if (appliedCondiments.Count > 0)
            {
                label += $"\nCondiments: {string.Join(", ", GetCondimentNames())}";
            }

            Handles.Label(transform.position + transform.forward * 0.5f, label, style);
        }
        else
        {
            style.normal.textColor = Color.red;
            Handles.Label(transform.position + transform.forward * 0.5f, "Ingredient needs data!", style);
        }
    }
#endif
}