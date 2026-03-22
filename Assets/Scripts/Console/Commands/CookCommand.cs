using UnityEngine;

[CreateAssetMenu(menuName = "Console Commands/Cook Held Item")]
/// cooks the currently held item if it is a cookable
public class CookCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        PlayerItemHolder holder = Object.FindFirstObjectByType<PlayerItemHolder>();
        if (holder == null)
        {
            Debug.LogWarning("[CookHeldItem] No PlayerItemHolder found.");
            return false;
        }

        Holdable held = holder.ItemInHand;
        if (held == null)
        {
            Debug.LogWarning("[CookHeldItem] Player is not holding anything.");
            return false;
        }

        if (!held.TryGetComponent<Ingredient>(out var ingredient))
        {
            Debug.LogWarning("[CookHeldItem] Held object is not an Ingredient.");
            return false;
        }

        if (ingredient.TryGetCookable(out var cookable) == null)
        {
            Debug.LogWarning($"[CookHeldItem] '{ingredient}' is not cookable.");
            return false;
        }

        // Default = cooked
        string mode = args.Length > 0 ? args[0].ToLower() : "cooked";

        switch (mode)
        {
            case "raw":
                cookable.ResetCook();
                break;

            case "burnt":
                cookable.ForceBurnt();
                break;

            case "cooked":
            default:
                cookable.ForceCooked();
                break;
        }

        Debug.Log($"[CookHeldItem] Set '{ingredient.name}' to {cookable.CookState}");
        return true;
    }
}