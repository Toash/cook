using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

/// <summary>
/// If player is in seat, input should drive the car.
/// </summary>
public class CarSeat : InteractableBase
{
    public List<InteractInfo> SeatInfos = new List<InteractInfo>();
    [ReadOnly]
    public Player PlayerInSeat;
    // where the player should snap to.
    public Transform SeatPosition;
    public Transform GetOutPosition;

    public event Action<PlayerController> GotInSeat;
    public event Action PlayerInSeatSeconaryInteraction;
    public event Action GotOutSeat;

    public void ConstraintInteract(InteractionContext context)
    {
        if (context.Type == InteractType.Primary)
        {
            context.Player.Controller.UnconstrainBody();
        }
        else if (context.Type == InteractType.Secondary)
        {
            PlayerInSeatSeconaryInteraction?.Invoke();
        }
    }

    public override void Interact(InteractionContext context)
    {
        if (PlayerInSeat != null)
        {
            // PlayerInSeat.Controller.UnconstrainBody();
            OnUnConstrained();
            context.Player.Controller.UnconstrainBody();
        }
        else
        {
            PlayerInSeat = context.Player;
            PlayerInSeat.Controller.ConstrainBody(new ConstrainedInfo(this, ConstrainType.Truck, SeatPosition, GetOutPosition));
            context.Player.Controller.ForceFirstPerson();

            GotInSeat?.Invoke(context.Player.Controller);

        }
    }
    public override List<InteractInfo> GetInteractInfos()
    {

        if (PlayerInSeat == null)
        {
            return HoverInteractInfo;
        }
        else
        {
            return SeatInfos;
        }

    }

    public void OnUnConstrained()
    {
        PlayerInSeat = null;
        GotOutSeat?.Invoke();
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (SeatPosition != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(SeatPosition.position, .2f);
        }

        if (GetOutPosition != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(GetOutPosition.position, .2f);
        }



    }

    public IInteractable GetInteractable()
    {
        throw new NotImplementedException();
    }
#endif
}