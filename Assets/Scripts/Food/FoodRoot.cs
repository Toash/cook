
using System.Collections.Generic;
using UnityEngine;

public class FoodRoot : MonoBehaviour
{
    public List<FoodIngredient> ingredients = new List<FoodIngredient>();


    public static FoodRoot CreateRootFromIngredient(FoodIngredient ingredient)
    {
        GameObject rootObj = new GameObject("Food root");
        FoodRoot foodRoot = rootObj.AddComponent<FoodRoot>();

        foodRoot.ingredients.Add(ingredient);
        ingredient.transform.SetParent(rootObj.transform);
        return foodRoot;
    }
    public void AddIngredient(FoodIngredient ingredient)
    {
        ingredients.Add(ingredient);
        ingredient.transform.SetParent(transform);
        ingredient.SetFoodRoot(this);
    }

    public void RemoveIngredient(FoodIngredient ingredient)
    {
        ingredients.Remove(ingredient);
        ingredient.transform.SetParent(null);
        ingredient.SetFoodRoot(null);

        if (transform.childCount == 0) Destroy(gameObject);

    }



}