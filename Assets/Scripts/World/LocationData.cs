using UnityEngine;

namespace Assets.Scripts.World
{
    /// <summary>
    /// Location that NPCS can go to
    /// </summary>
    [CreateAssetMenu(fileName = "Location", menuName = "World/Location")]
    public class LocationData : ScriptableObject
    {
        public string Name;

    }
}