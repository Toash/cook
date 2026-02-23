
using UnityEngine;

public enum FoodIngredientType
{
    BUN,
    PATTY,

}

public class FoodIngredient : MonoBehaviour
{
    public FoodIngredientType Type;



    // what food this ingredient belongs to, if any.
    public FoodRoot FoodRoot { get; private set; }



    public void SetFoodRoot(FoodRoot root)
    {
        this.FoodRoot = root;
    }



}