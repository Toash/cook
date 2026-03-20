using UnityEngine;

[CreateAssetMenu(menuName = "Console Commands/SubmitHeldOrder")]
public class SubmitHeldOrderCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        if (OrderManager.I == null)
        {
            Debug.LogError("[Console]: OrderManager not found");
            return false;
        }

        Player player = Object.FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError("[Console]: Player not found");
            return false;
        }

        PlayerItemHolder itemHolder = player.GetComponent<PlayerItemHolder>();
        if (itemHolder == null)
        {
            Debug.LogError("[Console]: PlayerItemHolder not found");
            return false;
        }

        OrderSubmissionResult result = OrderManager.I.TrySubmitFromPlayerHand(itemHolder);
        OrderManager.I.OrderEvaluated(result);

        itemHolder.TryDeleteHeldItem();

        Debug.Log($"[Console]: Submit held order result = {result.Status}");
        return result.Status == OrderSubmissionStatus.Success;
    }
}