using UnityEngine;

namespace Assets.Scripts.World
{
    /// <summary>
    /// A building in the world that npcs can enter at certain times
    /// </summary>
    public class Location : MonoBehaviour
    {
        [HideInInspector]
        public LocationData SO;
    }
}