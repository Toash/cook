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
    public override void Interact(InteractionContext context)
    {
        PlayerInSeat = context.Player;
        PlayerInSeat.Controller.ConstrainBody(new ConstrainedContext(this, SeatPosition, GetOutPosition));

    }

    public void OnUnConstrained()
    {
        PlayerInSeat = null;
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