
using System;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Represenets an actual ingredient in the world
/// </summary>
[RequireComponent(typeof(FoodSnapper))]
public class Ingredient : MonoBehaviour
{
    public IngredientType Type;

    private FoodSnapper snapper;



    // what food this ingredient belongs to, if any.
    public PreparedItem PreparedItem { get; private set; }

    void Awake()
    {
        snapper = GetComponent<FoodSnapper>();
    }

    void Start()
    {
        if (TryGetComponent<InteractableBase>(out var interactable))
        {
            interactable.HoverTooltip = Type.ToString();
        }
    }


    void OnEnable()
    {
        snapper.OnSnapEvent += OnSnapEvent;
        snapper.OnDetachedEvent += OnDetachedEvent;
    }
    void OnDisable()
    {
        snapper.OnSnapEvent -= OnSnapEvent;
        snapper.OnDetachedEvent -= OnDetachedEvent;
    }


    void OnSnapEvent(Ingredient other)
    {

        if (PreparedItem != null && other.PreparedItem != null)
        {
            // add to greater food root
            PreparedItem toAdd = PreparedItem.GetGreater(PreparedItem, other.PreparedItem);
            toAdd.AddIngredient(this);
            toAdd.AddIngredient(other);
        }
        else if (PreparedItem != null)
        {
            PreparedItem.AddIngredient(other);
        }
        else if (other.PreparedItem != null)
        {
            other.PreparedItem.AddIngredient(this);
        }
        else
        {
            // create new food root
            PreparedItem foodRoot = PreparedItem.CreateItemFromIngredient(this);
            foodRoot.AddIngredient(other);
        }

    }
    void OnDetachedEvent(Ingredient other)
    {
        // remove food root if it exists
        if (PreparedItem != null)
        {
            PreparedItem.RemoveIngredient(this);
        }

        if (other.PreparedItem != null)
        {
            other.PreparedItem.RemoveIngredient(this);
        }

    }




    public void SetFoodRoot(PreparedItem root)
    {
        this.PreparedItem = root;
    }

    public void RemoveFoodRoot()
    {
        transform.SetParent(null);
        SetFoodRoot(null);

    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up * .2f, "PreparedItem: " + PreparedItem);

    }
#endif

}