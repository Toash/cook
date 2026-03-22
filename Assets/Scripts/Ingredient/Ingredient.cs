
using System;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Represenets an actual ingredient in the world
/// </summary>
[RequireComponent(typeof(Snapper))]
[RequireComponent(typeof(Holdable))]
public class Ingredient : MonoBehaviour
{
    public IngredientData Data;


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
            if (TryGetComponent<Cookable>(out var cookable))
            {
                interactable.HoverTooltipData = new HoverTooltipData(transform, () =>
                {
                    return cookable.CookState.ToString() + $" {Data.Name}";
                });
            }
            else
            {

                interactable.HoverTooltipData = new HoverTooltipData(transform, Data.Name);
            }
        }
    }
    public Cookable TryGetCookable(out Cookable cookable)
    {
        return TryGetComponent<Cookable>(out cookable) ? cookable : null;
    }

    public override string ToString()
    {
        return Data.Name;
    }



#if UNITY_EDITOR
    GUIStyle style = new();
    void OnDrawGizmosSelected()
    {
        style.normal.textColor = Color.green;
        if (Data != null)
        {
            Handles.Label(transform.position + transform.forward * 0.5f, $"Ingredient: {Data.Name}", style);
        }
        else
        {
            style.normal.textColor = Color.red;
            Handles.Label(transform.position + transform.forward * 0.5f, $"Ingredient needs data!", style);

        }


    }
#endif

}