
using System;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Represenets an actual ingredient in the world
/// </summary>
[RequireComponent(typeof(Snapper))]
public class Ingredient : MonoBehaviour
{
    public IngredientData Data;



    // what food this ingredient belongs to, if any.
    public PreparedItem PreparedItem { get; private set; }

    void Start()
    {
        if (TryGetComponent<InteractableBase>(out var interactable))
        {
            interactable.HoverTooltip = Data.Type.ToString();
        }
    }


    public void SetPreparedItem(PreparedItem root)
    {
        this.PreparedItem = root;
    }

    public void RemovePreparedItem()
    {
        transform.SetParent(null);
        SetPreparedItem(null);

    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Handles.Label(transform.position + Vector3.up * .2f, "PreparedItem: " + PreparedItem);

    }
#endif

}