using Assets.Scripts.Vehicle;

public class FoodTruckServe : InteractableBase
{
    // public FoodTruck Truck;

    public override void Interact(InteractionContext context)
    {
        // if (Truck.CurrentState == TruckState.Stopped)
        // {
        //     Truck.TryServe();
        // }
        // else if (Truck.CurrentState == TruckState.Serving)
        // {
        //     Truck.StopServing();


        // }

    }

    // void OnEnable()
    // {
    //     Truck.EnteredState += OnEnteredState;
    // }
    // void OnDisable()
    // {
    //     Truck.EnteredState -= OnEnteredState;
    // }
    // void Start()
    // {
    //     OnEnteredState(null, Truck.CurrentState);
    // }


    // void OnEnteredState(TruckState prev, TruckState state)
    // {
    //     switch (state)
    //     {
    //         case TruckState.Serving:
    //             HoverTooltipData = new HoverTooltipData(transform, "Stop Serving");
    //             break;

    //         case TruckState.Stopped:
    //             HoverTooltipData = new HoverTooltipData(transform, "Start Serving");
    //             break;

    //         case TruckState.Driving:
    //             HoverTooltipData = new HoverTooltipData(transform, "Cannot Serve - Vehicle is driving.");
    //             break;

    //     }

    // }

}