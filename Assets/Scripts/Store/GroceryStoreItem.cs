using UnityEngine;
[CreateAssetMenu(fileName = "GroceryStoreItem", menuName = "Store/GroceryStoreItem")]
public class GroceryStoreItem : ScriptableObject
{
    public IngredientData Ingredient;
    public int Amount;

    public int Price;


}