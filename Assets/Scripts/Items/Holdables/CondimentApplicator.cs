using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CondimentApplicatorHoldable : Holdable
{

    public List<InteractInfo> CanApplyInfos = new()
    {
        new InteractInfo(InteractType.Primary, "Apply")
    };

    [SerializeField] private CondimentData condiment;

    public override bool OnPressedInteract(PlayerItemHolder holder, InteractionContext context)
    {
        if (context.Type != InteractType.Primary)
            return false;


        Ingredient ingredient = context.Player.Interaction.GetHoveredComponent<Ingredient>();
        if (ingredient == null)
            return false;

        ingredient.TryApplyCondiment(condiment);
        return true;
    }
    public override List<InteractInfo> GetHeldInteractInfos(PlayerItemHolder holder)
    {
        Ingredient ingredient = holder.Player.Interaction.GetHoveredComponent<Ingredient>();
        if (ingredient == null)
            return HeldInfos;

        if (ingredient.CanApplyCondiment(condiment))
            return CanApplyInfos;

        return HeldInfos;
    }
}