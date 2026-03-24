using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Console Commands/Order Supply")]
public class OrderSupplyConsoleCommand : ConsoleCommand
{

    [Tooltip("Delay, in in-game seconds, before delivery arrives.")]
    [SerializeField] private int defaultDeliveryDelaySeconds = 2;

    public override bool Process(string[] args)
    {
        if (DeliveryManager.I == null)
        {
            Debug.LogError("[OrderSupplyConsoleCommand]: No DeliveryManager found in scene.");
            return false;
        }

        if (args == null || args.Length < 2 || args.Length % 2 != 0)
        {
            Debug.LogWarning(
                "[OrderSupplyConsoleCommand]: Usage: order_supply <itemName> <amount> [itemName amount] ..."
            );
            return false;
        }

        SupplyOrder order = new SupplyOrder();

        for (int i = 0; i < args.Length; i += 2)
        {
            string itemName = args[i];

            if (!int.TryParse(args[i + 1], out int amount) || amount <= 0)
            {
                Debug.LogWarning(
                    $"[OrderSupplyConsoleCommand]: Invalid amount '{args[i + 1]}' for item '{itemName}'."
                );
                return false;
            }

            HoldableData itemData = HoldableSpawnDatabase.Instance.Get(itemName);

            if (itemData == null)
            {
                Debug.LogWarning(
                    $"[OrderSupplyConsoleCommand]: Could not find HoldableData for item '{itemName}'."
                );
                return false;
            }

            order.Items.Add(new SupplyEntry
            {
                Item = itemData,
                Amount = amount
            });
        }

        DeliveryManager.I.OrderSupplies(order, defaultDeliveryDelaySeconds);

        Debug.Log($"[OrderSupplyConsoleCommand]: Ordered {order.Items.Count} item type(s).");
        return true;
    }
}