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
    public Transform Seat;
    public Transform GetOut;
    public override void Interact(InteractionContext context)
    {
        PlayerInSeat = context.Player;
        PlayerInSeat.Controller.ConstrainBody(new ConstrainedContext(this, Seat, GetOut));

    }

    public void OnUnConstrained()
    {
        PlayerInSeat = null;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (Seat != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(Seat.position, .2f);
        }

        if (GetOut != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(GetOut.position, .2f);
        }



    }
#endif
}