using System.Collections.Generic;
using UnityEngine;
public class FoodTruckParkingSpotManager : MonoBehaviour
{
    public static FoodTruckParkingSpotManager Instance;

    private List<FoodTruckParking> spots = new();

    void Awake() => Instance = this;

    public void Register(FoodTruckParking spot) => spots.Add(spot);

    public FoodTruckParking GetNearest(Vector3 pos)
    {
        FoodTruckParking best = null;
        float bestDistSq = float.MaxValue;

        foreach (var spot in spots)
        {
            if (spot == null) continue;

            float distSq = (spot.transform.position - pos).sqrMagnitude;

            if (distSq < bestDistSq)
            {
                bestDistSq = distSq;
                best = spot;
            }
        }

        return best;
    }
}