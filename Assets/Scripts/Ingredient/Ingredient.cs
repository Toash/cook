
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

    public PhysicsGrabbable Grabbable { get; private set; }


    // what food this ingredient belongs to, if any.
    public PreparedItem PreparedItem { get; set; }
    public Snapper Snapper { get; private set; }



    void Awake()
    {
        Grabbable = GetComponent<PhysicsGrabbable>();
        Snapper = GetComponent<Snapper>();
    }
    void Start()
    {
        if (TryGetComponent<InteractableBase>(out var interactable))
        {
            interactable.HoverTooltip = Data.Type.ToString();
        }
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