using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.World
{
    /// <summary>
    /// Stores references to important locations in the map that should be globally accessible. 
    /// </summary>
    public class LocationManager : MonoBehaviour
    {
        public static LocationManager I;
        void Awake()
        {
            if (I != null && I != this)
            {
                Destroy(gameObject);
                return;
            }
            else if (I == null)
            {
                DontDestroyOnLoad(gameObject);
                I = this;
            }
        }

        [ShowInInspector, ReadOnly]
        private Dictionary<LocationData, Location> locationRegistry;


        public Location GetLocation(LocationData data)
        {
            if (locationRegistry.TryGetValue(data, out Location loc))
            {
                return loc;
            }
            Debug.LogError("[LocationManager]: Location does not exist!");
            return null;
        }

        void Start()
        {

            locationRegistry = new();
            Location[] locations = FindObjectsByType<Location>(FindObjectsSortMode.None);
            foreach (var location in locations)
            {
                if (location.SO == null)
                {
                    Debug.LogError("[LocationManager]: Scriptable Object not found on a location.");
                }

                locationRegistry.Add(location.SO, location);
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (locationRegistry == null) return;
            foreach (var kv in locationRegistry)
            {
                var location = kv.Value;
                if (location == null) continue;
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(location.transform.position, .5f);
                Handles.Label(location.transform.position, "Location: " + location.SO.Name);
            }
        }
#endif

    }
}