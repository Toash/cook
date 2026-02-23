
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// source of truth for a "food" that an npc would receive.
/// </summary>
public class FoodQuality
{
    // determine based on placements of each individual ingredient
}

// [RequireComponent(typeof(Rigidbody))]
public class FoodRoot : MonoBehaviour
{
    public List<FoodIngredient> ingredients = new List<FoodIngredient>();


    public FoodQuality GetFoodQuality()
    {
        return null;
    }
    public static FoodRoot CreateRootFromIngredient(FoodIngredient ingredient)
    {
        GameObject rootObj = new GameObject("Food root");
        rootObj.transform.position = ingredient.transform.position;
        FoodRoot foodRoot = rootObj.AddComponent<FoodRoot>();

        // Rigidbody rb = rootObj.GetComponent<Rigidbody>();
        // rb.useGravity = false;
        // rb.isKinematic = true;
        // rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Collider col = rootObj.AddComponent<SphereCollider>();
        // col.isTrigger = true;



        foodRoot.ingredients.Add(ingredient);
        ingredient.transform.SetParent(rootObj.transform);


        return foodRoot;
    }
    public int IngredientCount()
    {
        return ingredients.Count;
    }
    public static FoodRoot GetGreater(FoodRoot a, FoodRoot b)
    {
        if (b.IngredientCount() > a.IngredientCount())
        {
            return b;
        }
        return a;

    }

    /// <summary>
    /// Adds ingredient to food root and deletes from existing food root if it already exists.
    /// </summary>
    /// <param name="ingredient"></param>
    public void AddIngredient(FoodIngredient ingredient)
    {
        if (ingredients.Contains(ingredient)) return;
        // remove from existing food root.
        if (ingredient.FoodRoot != null)
        {
            ingredient.FoodRoot.RemoveIngredient(ingredient);
        }

        ingredients.Add(ingredient);
        ingredient.transform.SetParent(transform);
        ingredient.SetFoodRoot(this);
    }

    public void RemoveIngredient(FoodIngredient ingredient)
    {
        ingredients.Remove(ingredient);
        ingredient.RemoveFoodRoot();

        if (ingredients.Count <= 1)
        {
            // remove the foodroot from the ingredient, and delete the food root
            FoodIngredient first = ingredients[0];
            first.RemoveFoodRoot();

            ingredients.Clear();
            Destroy(gameObject);
        }

    }



#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (ingredients.Count > 0)
        {
            GUIStyle style = new GUIStyle();
            // style.fontSize = 24;
            style.normal.textColor = Color.green;
            // Handles.Label(transform.position + Vector3.up * .6f, "Food root", style);
            string message = "Food root";
            message += "\nIngredient count: " + IngredientCount();
            Handles.Label(ingredients.First<FoodIngredient>().transform.position + Vector3.up * .6f, message, style);
        }
    }
#endif



}