
using UnityEngine;

public enum IngredientType
{
    // Foods
    BurgerBun,
    Patty,
    Hotdog,
    HotdogBun,
    Lettuce,
    Tomato
}
/// <summary>
/// Data for an ingredient
/// </summary>
[CreateAssetMenu(fileName = "IngredientData", menuName = "Items/IngredientData")]
public class IngredientData : HoldableData
{
    [Header("Ingredient Specific")]
    public IngredientType Type;
    public Texture2D Image;



}