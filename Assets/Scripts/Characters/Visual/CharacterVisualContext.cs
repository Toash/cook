
using UnityEngine;

/// <summary>
/// data for the visual to draw from to decide what to do visually.
/// </summary>
[System.Serializable]
public class CharacterVisualContext
{
    public Transform ForceLookAtTarget;
    public Player NearbyPlayer;
    public Transform FoodTarget;
    public bool IsEating;


}