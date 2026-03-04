using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// If player is in seat, input should drive the car.
/// </summary>
public class CarSeat : InteractableBase, IConstrainer
{
    [ReadOnly]
    public Player Player;
    // where the player should snap to.
    public Transform Seat;
    public Transform GetOut;
    public override void Interact(InteractionContext context)
    {
        Player = context.Player;
        Player.Controller.ConstrainBody(new ConstrainedContext(this, Seat, GetOut));

    }

    public void OnUnConstrained()
    {
        Player = null;
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