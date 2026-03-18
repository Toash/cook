using Assets.Scripts.Vehicle;
using UnityEngine;

[CreateAssetMenu(menuName = "Console Commands/Teleport Truck To Parking")]
public class TeleportTruckToParkingCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        if (FoodTruckParkingSpotManager.Instance == null)
        {
            Debug.LogWarning("No ParkingSpotManager found.");
            return false;
        }

        var truck = FindFirstObjectByType<FoodTruck>();
        if (truck == null)
        {
            Debug.LogWarning("No FoodTruck found.");
            return false;
        }

        var spot = FoodTruckParkingSpotManager.Instance.GetNearest(truck.transform.position);

        if (spot == null)
        {
            Debug.LogWarning("No parking spots available.");
            return false;
        }

        // truck.transform.SetPositionAndRotation(
        //     spot.transform.position,
        //     spot.transform.rotation
        // );

        truck.Rigidbody.MovePosition(spot.transform.position);
        truck.Rigidbody.MoveRotation(spot.transform.rotation);

        Debug.Log($"Teleported truck to {spot.name}");
        return true;
    }
}