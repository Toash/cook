
using System.Collections.Generic;
using System.Linq;
using GamingIsLove.Footsteps;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;


/// <summary>
/// A submittable food item, that exists in the world
/// </summary>
public class PreparedItem : MonoBehaviour
{
    public List<Ingredient> Ingredients = new List<Ingredient>();


    /// <summary>
    /// Adds ingredient to food root and deletes from existing food root if it already exists.
    /// </summary>
    /// <param name="ingredient"></param>
    public void AddIngredient(Ingredient ingredient)
    {
        if (Ingredients.Contains(ingredient)) return;
        // remove from existing food root.
        if (ingredient.PreparedItem != null)
        {
            ingredient.PreparedItem.RemoveIngredient(ingredient);
        }

        Ingredients.Add(ingredient);
        ingredient.transform.SetParent(transform);
        ingredient.SetFoodRoot(this);
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        Ingredients.Remove(ingredient);
        ingredient.RemoveFoodRoot();

        if (Ingredients.Count <= 1)
        {
            // remove the foodroot from the ingredient, and delete the food root
            Ingredient first = Ingredients[0];
            first.RemoveFoodRoot();

            Ingredients.Clear();
            Destroy(gameObject);
        }

    }
    public static PreparedItem CreateItemFromIngredient(Ingredient ingredient)
    {
        GameObject rootObj = new GameObject("Prepared Item");
        rootObj.transform.position = ingredient.transform.position;
        PreparedItem foodRoot = rootObj.AddComponent<PreparedItem>();

        foodRoot.Ingredients.Add(ingredient);
        ingredient.transform.SetParent(rootObj.transform);


        return foodRoot;
    }
    public int IngredientCount()
    {
        return Ingredients.Count;
    }
    public static PreparedItem GetGreater(PreparedItem a, PreparedItem b)
    {
        if (b.IngredientCount() > a.IngredientCount())
        {
            return b;
        }
        return a;

    }




    // #if UNITY_EDITOR
    //     void OnDrawGizmos()
    //     {
    //         if (Ingredients.Count > 0)
    //         {
    //             GUIStyle style = new GUIStyle();
    //             // style.fontSize = 24;
    //             style.normal.textColor = Color.green;
    //             // Handles.Label(transform.position + Vector3.up * .6f, "Food root", style);
    //             string message = "Food root";
    //             message += "\nIngredient count: " + IngredientCount();
    //             Handles.Label(Ingredients.First<Ingredient>().transform.position + Vector3.up * .6f, message, style);
    //         }
    //     }
    // #endif



}