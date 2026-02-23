
using System;
using UnityEditor;
using UnityEngine;


/// <summary>
/// Represenets an actual ingredient in the world
/// </summary>
[RequireComponent(typeof(FoodSnapper))]
public class FoodIngredient : MonoBehaviour
{
    public FoodIngredientType Type;

    private FoodSnapper snapper;



    // what food this ingredient belongs to, if any.
    public FoodRoot FoodRoot { get; private set; }

    void Awake()
    {
        snapper = GetComponent<FoodSnapper>();
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


    void OnSnapEvent(FoodIngredient other)
    {

        if (FoodRoot != null && other.FoodRoot != null)
        {
            // add to greater food root
            FoodRoot toAdd = FoodRoot.GetGreater(FoodRoot, other.FoodRoot);
            toAdd.AddIngredient(this);
            toAdd.AddIngredient(other);
        }
        else if (FoodRoot != null)
        {
            FoodRoot.AddIngredient(other);
        }
        else if (other.FoodRoot != null)
        {
            other.FoodRoot.AddIngredient(this);
        }
        else
        {
            // create new food root
            FoodRoot foodRoot = FoodRoot.CreateRootFromIngredient(this);
            foodRoot.AddIngredient(other);
        }

    }
    void OnDetachedEvent(FoodIngredient other)
    {
        // remove food root if it exists
        if (FoodRoot != null)
        {
            FoodRoot.RemoveIngredient(this);
        }

        if (other.FoodRoot != null)
        {
            other.FoodRoot.RemoveIngredient(this);
        }

    }

    public FoodRoot GetFoodRoot()
    {
        return this.FoodRoot;
    }



    public void SetFoodRoot(FoodRoot root)
    {
        this.FoodRoot = root;
    }

    public void RemoveFoodRoot()
    {
        transform.SetParent(null);
        SetFoodRoot(null);

    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.Label(transform.position + Vector3.up * .3f, "Foot root: " + FoodRoot);

    }
#endif

}