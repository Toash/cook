using UnityEngine;

[CreateAssetMenu(fileName = "IngredientSnapSettings", menuName = "Food/IngredientSnapSettings")]
public class SnapperSettings : ScriptableObject
{
    public bool Infinity = false;
    public float BreakForce;
    public float BreakTorque;

}