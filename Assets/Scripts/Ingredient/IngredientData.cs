
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
public class IngredientData : ItemData
{
    public IngredientType Type;
    public Texture2D Image;



}