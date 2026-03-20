using System;
using System.Collections.Generic;
using Assets.Scripts.Vehicle;
using Sirenix.OdinInspector;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

/// <summary>
/// If player is in seat, input should drive the car.
/// </summary>
public class CarSeat : InteractableBase
{
    public FoodTruck Truck;
    public List<InteractInfo> CantParkSeatInfos = new List<InteractInfo>();
    public List<InteractInfo> CanParkSeatInfos = new List<InteractInfo>();
    [ReadOnly]
    public Player PlayerInSeat;
    // where the player should snap to.
    public Transform SeatPosition;
    public Transform GetOutPosition;

    public event Action<PlayerController> GotInSeat;
    public event Action PlayerInSeatSeconaryInteraction;
    public event Action GotOutSeat;

    void OnEnable()
    {
        Truck.EnteredParkingSpot += OnEnteredParkingSpot;
        Truck.LeftParkingSpot += OnLeftParkingSpot;
    }
    void OnDisable()
    {
        Truck.EnteredParkingSpot -= OnEnteredParkingSpot;
        Truck.LeftParkingSpot -= OnLeftParkingSpot;
    }
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
        if (context.Type == InteractType.Primary)
        {
            if (PlayerInSeat != null)
            {
                context.Player.Controller.UnconstrainBody();
                context.Player.Controller.ForceFirstPerson();
                GotOutSeat?.Invoke();

                PlayerInSeat = null;
            }
            else
            {
                PlayerInSeat = context.Player;

                PlayerInSeat.Controller.ConstrainBody(new ConstrainedInfo(this, ConstrainType.Truck, SeatPosition, GetOutPosition));
                // context.Player.Controller.ForceThirdPerson();

                GotInSeat?.Invoke(context.Player.Controller);
            }
        }
        else if (context.Type == InteractType.Secondary)
        {
            PlayerInSeatSeconaryInteraction?.Invoke();
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
            if (Truck.CanPark())
            {
                return CanParkSeatInfos;
            }
            return CantParkSeatInfos;
        }

    }


    void OnEnteredParkingSpot()
    {
        if (PlayerInSeat == null) return;
        PlayerInSeat.Interaction.InteractablePoll(this);
    }
    void OnLeftParkingSpot()
    {
        if (PlayerInSeat == null) return;
        PlayerInSeat.Interaction.InteractablePoll(this);
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