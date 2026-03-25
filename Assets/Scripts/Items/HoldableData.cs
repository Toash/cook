

using UnityEngine;

/// <summary>
/// Base class for item data
/// </summary>
[CreateAssetMenu(fileName = "HoldableData", menuName = "Items/HoldableData")]
public class HoldableData : ScriptableObject
{
    public string Name;
    public Texture2D Image;
    public Holdable Prefab;

    [Header("Audio")]
    public AudioDefinition PlaceSound;
    public AudioDefinition PickUpSound;

    void OnValidate()
    {
        if (PlaceSound == null)
        {
            PlaceSound = Resources.Load<AudioDefinition>("ScriptableObjects/AudioDefinition/DefaultPlace");
        }
        if (PickUpSound == null)
        {
            PickUpSound = Resources.Load<AudioDefinition>("ScriptableObjects/AudioDefinition/DefaultPickup");
        }
    }





}