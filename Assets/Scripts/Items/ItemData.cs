

using UnityEngine;

/// <summary>
/// Base class for item data
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    public string Name;
    public Holdable HoldablePrefab;


}