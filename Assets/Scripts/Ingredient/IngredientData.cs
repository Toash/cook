
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
[CreateAssetMenu(fileName = "IngredientData", menuName = "Food/IngredientData")]
public class IngredientData : ScriptableObject
{
    public IngredientType Type;
    public string Name;
    public Texture2D Image;


}