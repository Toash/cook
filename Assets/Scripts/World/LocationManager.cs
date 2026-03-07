using Sirenix.OdinInspector;
using System.Collections.Generic;
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

        [ShowInInspector]
        public Dictionary<LocationData, Location> Locations;


        public Location GetLocation(LocationData data)
        {
            if (Locations.TryGetValue(data, out var loc))
            {
                return loc;
            }
            Debug.LogError("[LocationManager]: Location does not exist!");
            return null;
        }

        void Start()
        {
            foreach (var kv in Locations)
            {
                Location location = kv.Value;
                LocationData locationData = kv.Key;
                location.SO = locationData;
            }
        }


    }
}