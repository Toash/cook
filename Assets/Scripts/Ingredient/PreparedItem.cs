
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


/// <summary>
/// A submittable food item (what the player makes to try and match a menu item), that exists in the world<br/>
/// A PreparedItem should not exist without an OrderContainer. <br/>
/// 
/// Is made up of ingredients, position is the first ingredient.
/// </summary>
public class PreparedItem
{
    public List<Ingredient> Ingredients = new List<Ingredient>();
    public event Action IngredientsChanged;
    public override string ToString()
    {
        string ret = $"PreparedItem with {Ingredients.Count} ingredients.\n";
        foreach (var ingredient in Ingredients)
        {
            ret += $"  - {ingredient}\n";
        }
        return ret;
    }
    public int IngredientCount()
    {
        return Ingredients.Count;
    }
    void OnDestroy()
    {
        DestroyIngredients();
    }


    void OnIngredientAttachedToIngredient(Snapper otherSnapper)
    {
        Debug.Log("[PreparedItem]: Ingredient attached: " + otherSnapper);
        var snappers = otherSnapper.GetSnapperChildrenRecursive();
        foreach (var snapper in snappers)
        {
            if (snapper.TryGetComponent<Ingredient>(out var ingredient))
            {
                if (Ingredients.Contains(ingredient))
                {
                    AddIngredient(ingredient);
                }
            }
        }
    }

    void OnIngredientDetachedFromIngredient(Snapper otherSnapper)
    {
        Debug.Log("[PreparedItem]: Ingredient detached: " + otherSnapper);
        var snappers = otherSnapper.GetSnapperChildrenRecursive();
        foreach (var snapper in snappers)
        {
            if (snapper.TryGetComponent<Ingredient>(out var ingredient))
            {
                if (Ingredients.Contains(ingredient))
                {
                    RemoveIngredient(ingredient);
                }
            }
        }
    }


    /// <summary>
    /// creates a prepared item and sets the position to the first ingredient.
    /// </summary>
    /// <param name="ingredients"></param>
    /// <returns></returns>
    public static PreparedItem Create(List<Ingredient> ingredients)
    {
        if (ingredients.Count == 0) return null;

        PreparedItem preparedItem = new();
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
        IngredientsChanged?.Invoke();

        ingredient.Snapper.OnChildSnapped += OnIngredientAttachedToIngredient;
        ingredient.Snapper.OnChildDetached += OnIngredientDetachedFromIngredient;

        ingredient.PreparedItem = this;
        Debug.Log("[PreparedItem]: Added ingredient: " + ingredient);
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
        IngredientsChanged?.Invoke();

        ingredient.transform.SetParent(null);
        ingredient.PreparedItem = null;

        ingredient.Snapper.OnChildSnapped -= OnIngredientAttachedToIngredient;
        ingredient.Snapper.OnChildDetached -= OnIngredientDetachedFromIngredient;
        Debug.Log("[PreparedItem]: Removed ingredient: " + ingredient);
    }

    /// <summary>
    /// Destroys all of the ingredients in this PreparedItem
    /// </summary>
    void DestroyIngredients()
    {
        for (int i = Ingredients.Count - 1; i >= 0; i--)
        {
            var ingredient = Ingredients[i];
            RemoveIngredient(ingredient);
        }

    }

    /// <summary>
    /// Removes all ingredients from the PreparedItem, deletes the PreparedItem
    /// </summary>
    public void Disband()
    {
        for (int i = Ingredients.Count - 1; i >= 0; i--)
        {
            RemoveIngredient(Ingredients[i]);
        }
    }



}