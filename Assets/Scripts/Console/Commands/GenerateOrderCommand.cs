using UnityEngine;

[CreateAssetMenu(menuName = "Console Commands/GenerateOrder")]
public class GenerateOrderCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        if (OrderManager.I == null)
        {
            Debug.LogError("[Console]: OrderManager not found");
            return false;
        }

        OrderStation station = FindFirstObjectByType<OrderStation>();
        NPC npc = null;

        var prop = OrderManager.I.GenerateRandomOrderProposition(station, npc);
        OrderManager.I.ProposeOrder(prop);

        Debug.Log("[Console]: Generated random order proposition (NPC = null)");

        return true;
    }
}