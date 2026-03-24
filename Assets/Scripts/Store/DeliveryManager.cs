using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager I { get; private set; }

    [SerializeField] private Transform dropoffPoint;
    [SerializeField] private float spawnRadius = 1f;
    [SerializeField] private float verticalSpawnOffset = 0.25f;

    private readonly List<PendingDelivery> pendingDeliveries = new();

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
    }

    private void Start()
    {
        TimeManager.I.MinuteChanged += OnMinuteChanged;
    }

    private void OnDestroy()
    {
        TimeManager.I.MinuteChanged -= OnMinuteChanged;
    }

    public void OrderSupplies(SupplyOrder order, int deliveryDelaySeconds)
    {
        if (order == null)
        {
            Debug.LogWarning("[DeliveryManager]: OrderSupplies called with null order.");
            return;
        }

        if (dropoffPoint == null)
        {
            Debug.LogError("[DeliveryManager]: Dropoff point is null.", this);
            return;
        }

        int currentTime = TimeManager.I.CurrentTimeSeconds;
        int arrivalTime = currentTime + deliveryDelaySeconds;

        pendingDeliveries.Add(new PendingDelivery
        {
            Order = order,
            ArrivalTimeSeconds = arrivalTime
        });
        Debug.Log($"[DeliveryManager]: Order placed. Will arrive in {deliveryDelaySeconds} seconds");

    }

    private void OnMinuteChanged(int hour, int minute)
    {
        if (TimeManager.I == null) return;

        int currentTime = TimeManager.I.CurrentTimeSeconds;

        for (int i = pendingDeliveries.Count - 1; i >= 0; i--)
        {
            PendingDelivery pending = pendingDeliveries[i];

            if (currentTime >= pending.ArrivalTimeSeconds)
            {
                SpawnOrder(pending.Order);
                pendingDeliveries.RemoveAt(i);
            }
        }
    }

    private void SpawnOrder(SupplyOrder order)
    {
        foreach (SupplyEntry entry in order.Items)
        {
            if (entry.Item == null || entry.Amount <= 0)
                continue;

            Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = dropoffPoint.position + new Vector3(offset2D.x, verticalSpawnOffset, offset2D.y);
            Quaternion spawnRot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);

            HoldableContainerManager.I.SpawnContainer(
                entry.Item,
                entry.Amount,
                spawnPos,
                spawnRot
            );
        }
    }

}
