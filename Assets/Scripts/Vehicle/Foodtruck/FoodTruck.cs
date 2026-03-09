using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Vehicle
{
    public enum TruckState
    {
        Stopped,
        Driving,
        Serving
    }
    [RequireComponent(typeof(Rigidbody))] // used to check speed
    public class FoodTruck : MonoBehaviour
    {
        public CarSeat Seat;
        private Rigidbody rb;
        void OnValidate()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
        }
        public bool IsServing { get; private set; }

        /// <summary>
        /// Spots NPCS wait to order something
        /// </summary>
        public OrderLine OrderLine;

        /// <summary>
        /// Spots NPCS wait while an order is being made 
        /// </summary>
        public OrderWaitingSpot WaitingSpot;

        /// <summary>
        /// Spot Player submits Made orders, and where NPCS pick up their orders.
        /// </summary>
        public SingleOrderSubmissionArea OrderSubmissionArea;

        TruckState currentState;


        void OnEnable()
        {
            Seat.GotInSeat += OnGotInSeat;
            Seat.GotOutSeat += OnGotOutSeat;
        }

        void OnDisable()
        {
            Seat.GotInSeat -= OnGotInSeat;
            Seat.GotOutSeat -= OnGotOutSeat;
        }

        void OnGotInSeat()
        {
            StartDriving();
        }
        void OnGotOutSeat()
        {
            StopDriving();
        }



        public void SetTruckState(TruckState state)
        {
            switch (state)
            {
                case TruckState.Stopped:
                    break;
                case TruckState.Driving:
                    break;
                case TruckState.Serving:
                    break;

            }
            currentState = state;

        }

        /// <summary>
        /// Start accepting customers
        /// </summary>
        /// <returns></returns>
        public bool TryStartServe()
        {
            if (GetTruckSpeed() > .1f) return false;
            if (currentState != TruckState.Stopped) return false;
            currentState = TruckState.Serving;
            return true;
        }

        void StartDriving()
        {
            currentState = TruckState.Driving;
        }
        void StopDriving()
        {
            currentState = TruckState.Stopped;
        }

        // public
        public float GetTruckSpeed()
        {
            return rb.linearVelocity.magnitude;
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            Handles.Label(transform.position, "Speed: " + GetTruckSpeed());

        }
#endif

    }
}