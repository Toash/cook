using System;
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

        public event Action<TruckState> EnteredState;
        public event Action StoppedServing;
        void OnValidate()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }
        }
        public bool IsServing
        {
            get => currentState == TruckState.Serving;
        }


        public OrderLine OrderLine;

        public OrderWaitingSpot WaitingSpot;

        public SingleOrderSubmissionArea OrderSubmissionArea;

        public TruckState CurrentState
        {
            get => currentState;
            set
            {
                switch (value)
                {
                    case TruckState.Stopped:
                        break;
                    case TruckState.Driving:
                        break;
                    case TruckState.Serving:
                        break;

                }
                if (currentState == TruckState.Serving && value != TruckState.Serving) StoppedServing?.Invoke();

                currentState = value;
                EnteredState?.Invoke(value);


            }
        }
        private TruckState currentState;


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



        // public void SetTruckState(TruckState newState)
        // {
        //     switch (newState)
        //     {
        //         case TruckState.Stopped:
        //             break;
        //         case TruckState.Driving:
        //             break;
        //         case TruckState.Serving:
        //             break;

        //     }
        //     CurrentState = newState;
        //     EnteredState?.Invoke(newState);

        // }

        /// <summary>
        /// Start accepting customers
        /// </summary>
        /// <returns></returns>
        public bool TryStartServe()
        {
            if (CurrentState != TruckState.Stopped) return false;
            if (IsMoving()) return false;

            // TODO bounds checking

            CurrentState = TruckState.Serving;
            return true;
        }
        public void StopServing()
        {
            CurrentState = TruckState.Stopped;

        }

        void StartDriving()
        {
            CurrentState = TruckState.Driving;
        }
        void StopDriving()
        {
            CurrentState = TruckState.Stopped;
        }

        // public
        public float GetTruckSpeed()
        {
            return rb.linearVelocity.magnitude;
        }
        public bool IsMoving()
        {
            if (GetTruckSpeed() > .1f) return true;
            return false;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            string message = "Truck Speed: " + GetTruckSpeed() + "\n";
            message += "Truck State: " + CurrentState.ToString();
            Handles.Label(transform.position, message);

        }
#endif

    }
}