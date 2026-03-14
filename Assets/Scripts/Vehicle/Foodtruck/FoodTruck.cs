using System;
using IngameDebugConsole;
using Sirenix.OdinInspector;
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
    public class FoodTruck : MonoBehaviour, IVelocityProvider
    {
        public SingleOrderSubmissionArea OrderSubmissionArea;
        public CarSeat Seat;
        private Rigidbody rb;
        public FoodTruckInfluence Influence;
        public FoodTruckCollider TruckCollider;

        public TruckParking CurrentParkingSpot { get; private set; }
        [ShowInInspector, ReadOnly]
        private TruckParking overlappedParkingSpot;

        public event Action EnteredParkingSpot;
        public event Action LeftParkingSpot;

        public event Action<TruckState> EnteredState;
        public event Action StoppedServing;
        public bool IsServing
        {
            get => currentState == TruckState.Serving && CurrentParkingSpot != null;
        }
        void OnValidate()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

        }
        void Start()
        {
            Influence.gameObject.SetActive(false);
            TruckCollider.EnteredParking += OnEnteredParking;
            TruckCollider.ExitedParking += OnExitedParking;

            Seat.PlayerInSeatSeconaryInteraction += TryServe;

        }

        void OnDestroy()
        {
            TruckCollider.EnteredParking -= OnEnteredParking;
            TruckCollider.ExitedParking -= OnExitedParking;

        }


        void OnEnteredParking(TruckParking parking)
        {
            overlappedParkingSpot = parking;

        }
        void OnExitedParking(TruckParking parking)
        {
            overlappedParkingSpot = null;
            if (CurrentParkingSpot != null)
            {
                CurrentParkingSpot = null;
            }

        }


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
                if (value == TruckState.Serving && CurrentParkingSpot == null) return;

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



        void OnGotInSeat(PlayerController playerController)
        {
            playerController.CurrentVelocityProvider = this;
            StartDriving();
        }
        void OnGotOutSeat()
        {
            StopDriving();
        }




        /// <summary>
        /// Start accepting customers
        /// </summary>
        /// <returns></returns>
        [Button]
        public void TryServe()
        {
            // if (CurrentState != TruckState.Stopped) return false;
            // if (IsMoving()) return false;

            if (overlappedParkingSpot == null) return;
            if (Seat.PlayerInSeat == null) return;

            Debug.Log("[FoodTruck]: Serving");


            // parent the player to the truck so it moves with it.

            // Seat.PlayerInSeat.transform.SetParent(transform);
            rb.MovePosition(overlappedParkingSpot.transform.position);
            rb.MoveRotation(overlappedParkingSpot.transform.rotation);
            // Seat.PlayerInSeat.transform.SetParent(null);


            CurrentParkingSpot = overlappedParkingSpot;
            CurrentState = TruckState.Serving;
            return;
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

        public Vector3 GetVelocity()
        {
            return rb.linearVelocity;
        }
#endif

    }
}