using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains information what a food contains.
/// </summary>
[CreateAssetMenu(fileName = "Recipe", menuName = "Food/Recipe")]
public class Recipe : ScriptableObject
{
    public List<IngredientRequirement> requirements = new List<IngredientRequirement>();

}