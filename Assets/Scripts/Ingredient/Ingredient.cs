
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

    public Snapper Snapper { get; private set; }


    // what food this ingredient belongs to, if any.
    public PreparedItem PreparedItem { get; set; }

    void Awake()
    {
        Snapper = GetComponent<Snapper>();
        Snapper.SetJointType(JointType.Food);
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
    void OnDrawGizmosSelected()
    {
        Handles.Label(transform.position + Vector3.up * .2f, "PreparedItem: " + PreparedItem);

    }
#endif

}