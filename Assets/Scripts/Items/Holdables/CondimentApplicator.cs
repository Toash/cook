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


        var condiment = context.Player.Interaction.GetHoveredComponent<CondimentReceiver>();
        if (condiment == null)
            return false;

        condiment.TryApplyCondiment(this.condiment);
        return true;
    }
    public override List<InteractInfo> GetHeldInteractInfos(PlayerItemHolder holder)
    {
        var condiment = holder.Player.Interaction.GetHoveredComponent<CondimentReceiver>();
        if (condiment == null)
            return HeldInfos;

        if (condiment.CanApplyCondiment(this.condiment))
            return CanApplyInfos;

        return HeldInfos;
    }
}