
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
public class PreparedItem : MonoBehaviour
{
    public List<Ingredient> Ingredients = new List<Ingredient>();
    public event System.Action IngredientDetached;
    public int IngredientCount()
    {
        return Ingredients.Count;
    }
    void OnDestroy()
    {
        DestroyIngredients();
    }


    void OnIngredientAttachedToIngredient(SnapConnection connection)
    {
        Debug.Log("[PreparedItem]: Ingredient attached: " + connection);
        // add to prepared item if snapper has an ingredient
        if (connection.This.TryGetComponent<Ingredient>(out var ingredient))
        {
            if (ingredient.PreparedItem == null)
            {
                AddIngredient(ingredient);
            }
        }
        if (connection.Other.TryGetComponent<Ingredient>(out var thisIngredient))
        {
            if (thisIngredient.PreparedItem == null)
            {
                AddIngredient(thisIngredient);
            }
        }
    }

    /// <summary>
    /// Handles which group should still be a PreparedItem when an ingredient gets detached.
    /// </summary>
    /// <param name="connection"></param>
    void OnIngredientDetachedFromIngredient(SnapConnection connection)
    {
        // get two groups
        //      one group that includes the container(may just only be the container), keep prepared item
        //      another group that doesnt include hte container, remove ingredients from the prepared item (loop).

        if (connection.Other.TryGetComponent<Ingredient>(out var otherIngredient))
        {
            // which group contains the container?
            List<Snapper> thisSide = connection.This.GetSnapperGroup();
            List<Snapper> otherSide = connection.Other.GetSnapperGroup();

            // remove ingredients on the group that does not contain the container
            if (OrderContainer.ContainsContainerInGroup(thisSide))
            {
                foreach (var snapper in otherSide)
                {
                    if (snapper.TryGetComponent<Ingredient>(out var ingredient))
                    {
                        RemoveIngredient(ingredient);
                    }
                }
            }
            else if (OrderContainer.ContainsContainerInGroup(otherSide))
            {
                foreach (var snapper in thisSide)
                {
                    if (snapper.TryGetComponent<Ingredient>(out var ingredient))
                    {
                        RemoveIngredient(ingredient);
                    }
                }
            }
            else
            {
                Debug.LogError("[PreparedItem]: When detaching ingredient, could not find OrderContainer in either group. A PreparedItem should belong to an OrderContainer.");
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

        ingredient.Snapper.OnSnap += OnIngredientAttachedToIngredient;
        ingredient.Snapper.OnDetached += OnIngredientDetachedFromIngredient;

        // ingredient.SetPreparedItem(this);
        ingredient.PreparedItem = this;
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
        // ingredient.RemovePreparedItem();

        ingredient.transform.SetParent(null);
        ingredient.PreparedItem = null;

        ingredient.Snapper.OnSnap -= OnIngredientAttachedToIngredient;
        ingredient.Snapper.OnDetached -= OnIngredientDetachedFromIngredient;


        if (Ingredients.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

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
            Destroy(ingredient.gameObject);
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

        Destroy(gameObject);
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