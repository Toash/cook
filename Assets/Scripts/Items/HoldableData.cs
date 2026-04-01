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

    [Header("Containers")]
    [Tooltip("Prefab used when this item is spawned inside a container.")]
    public HoldableContainer ContainerPrefab;

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

        if (ContainerPrefab == null)
        {
            ContainerPrefab = Resources.Load<HoldableContainer>("Prefabs/Holdables/Containers/GenericHoldableContainer");
        }
    }
}