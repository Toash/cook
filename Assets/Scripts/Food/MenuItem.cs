using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains information what a food contains.
/// 
/// When a player makes a food ingame, it gets checked to see what type of MenuItem it is.
/// </summary>
[CreateAssetMenu(fileName = "MenuItem", menuName = "Food/MenuItem")]
public class MenuItem : ScriptableObject
{
    public List<IngredientRequirement> requirements = new List<IngredientRequirement>();

}