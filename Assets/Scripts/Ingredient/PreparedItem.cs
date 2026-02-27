
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


/// <summary>
/// A submittable food item (what the player makes to try and match a menu item), that exists in the world<br\>
/// 
/// Is made up of ingredients, position is the first ingredient.
/// </summary>
public class PreparedItem : MonoBehaviour
{
    public List<Ingredient> Ingredients = new List<Ingredient>();

    public event System.Action<PreparedItem> Destroyed;


    void OnDestroy()
    {
        Destroyed?.Invoke(this);

    }
    /// <summary>
    /// creates a prepared item and sets the position to the first ingredient.
    /// </summary>
    /// <param name="ingredients"></param>
    /// <returns></returns>
    public static PreparedItem Create(List<Ingredient> ingredients)
    {
        if (ingredients.Count == 0) return null;
        GameObject obj = new GameObject("Prepared Item");
        obj.transform.position = ingredients.First().transform.position;
        PreparedItem preparedItem = obj.AddComponent<PreparedItem>();
        preparedItem.AddIngredients(ingredients);

        Debug.Log("[PreparedItem]: Creating with " + ingredients.Count + " ingredients.");

        return preparedItem;
    }

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
        ingredient.SetPreparedItem(this);
    }
    public void AddIngredients(List<Ingredient> ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            AddIngredient(ingredient);
        }

    }

    /// <summary>
    /// Removes an ingredient from the prepared item, deletes this if there are no more ingredients left.
    /// </summary>
    /// <param name="ingredient"></param>
    public void RemoveIngredient(Ingredient ingredient)
    {
        if (!Ingredients.Remove(ingredient)) return;
        ingredient.RemovePreparedItem();

        if (Ingredients.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

    }

    /// <summary>
    /// Removes all ingredients from the PreparedItem, deletes the PreparedItem
    /// 
    /// </summary>
    public void Disband()
    {
        foreach (var ingredient in Ingredients)
        {
            ingredient.RemovePreparedItem();
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
    public void Merge(PreparedItem other)
    {
        // loop thorugh prepared items from other, and add to this.
        foreach (var ing in other.Ingredients)
        {
            AddIngredient(ing);
        }
        Destroy(other.gameObject);

    }




#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (Ingredients.Count > 0)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.green;
            style.fontStyle = FontStyle.Bold;
            // Handles.Label(transform.position + Vector3.up * .6f, "Food root", style);
            string message = "PreparedItem";
            message += "\nIngredient count: " + IngredientCount();
            Handles.Label(Ingredients.First<Ingredient>().transform.position + Vector3.up * .6f, message, style);
        }
    }
#endif



}