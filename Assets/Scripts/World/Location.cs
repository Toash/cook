using Sirenix.OdinInspector;
using UnityEngine;

namespace Assets.Scripts.World
{
    /// <summary>
    /// A building in the world that npcs can enter at certain times
    /// </summary>
    public class Location : MonoBehaviour
    {
        /// <summary>
        /// The data object that describes this location. Used to query locations
        /// </summary>
        public LocationData SO;
    }
}