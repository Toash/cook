using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Food that the player sells, and what customers can order
/// </summary>
[CreateAssetMenu(fileName = "MenuItem", menuName = "Food/MenuItem")]
public class MenuItem : ScriptableObject
{
    public string Name;
    // what is in the order
    public List<IngredientRequirement> requirements = new List<IngredientRequirement>();

    public static MenuItem GetRandomMenuItem()
    {
        MenuItem[] items = Resources.LoadAll<MenuItem>("ScriptableObjects/MenuItems");
        MenuItem item = items[Random.Range(0, items.Length)];
        Debug.Log("Got random menu item: " + item.Name);
        return item;
    }
}