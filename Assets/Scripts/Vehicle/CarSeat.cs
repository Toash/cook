using System;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// If player is in seat, input should drive the car.
/// </summary>
public class CarSeat : InteractableBase, IConstrainer
{
    [ReadOnly]
    public Player PlayerInSeat;
    // where the player should snap to.
    public Transform SeatPosition;
    public Transform GetOutPosition;

    public event Action GotInSeat;
    public event Action GotOutSeat;


    public override void Interact(InteractionContext context)
    {
        PlayerInSeat = context.Player;
        PlayerInSeat.Controller.ConstrainBody(new ConstrainedContext(this, ConstrainType.Truck, SeatPosition, GetOutPosition));
        context.Player.Controller.ForceFirstPerson();

        GotInSeat?.Invoke();
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
#endif
}