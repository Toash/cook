using UnityEngine;

namespace Assets.Scripts.World
{
    /// <summary>
    /// Scriptable object for buildings. Can be used to get buildings
    /// </summary>
    [CreateAssetMenu(fileName = "Location", menuName = "World/Location")]
    public class LocationData : ScriptableObject
    {
        public string Name;

    }
}