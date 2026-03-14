using UnityEngine;

namespace Assets.Scripts.World
{
    /// <summary>
    /// Data object for a physical location in the world
    /// </summary>
    [CreateAssetMenu(fileName = "Location", menuName = "World/Location")]
    public class LocationData : ScriptableObject
    {
        public string Name;

    }
}