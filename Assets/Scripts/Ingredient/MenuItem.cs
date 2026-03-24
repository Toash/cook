using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Food that the player sells, and what customers can order
/// </summary>
namespace Assets.Scripts.Ingredient.MenuItem
{

    [CreateAssetMenu(fileName = "MenuItem", menuName = "Food/MenuItem")]
    public class OrderMenuItem : ScriptableObject
    {
        public string Name;
        // what is in the order
        public List<IngredientRequirement> Requirements = new List<IngredientRequirement>();
        public float BasePrice = 10;

        /// <summary>
        /// Returns counts of ingredients in a menu item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Dictionary<IngredientData, int> Consolidate(OrderMenuItem item)
        {
            var map = new Dictionary<IngredientData, int>();

            foreach (IngredientRequirement req in item.Requirements)
            {
                if (!map.ContainsKey(req.Data))
                    map[req.Data] = 0;

                map[req.Data] += req.Count;
            }

            return map;
        }
    }
}