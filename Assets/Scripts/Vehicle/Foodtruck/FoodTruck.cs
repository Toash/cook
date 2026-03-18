using System;
using IngameDebugConsole;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

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
        public Rigidbody Rigidbody { get; private set; }
        public FoodTruckInfluence Influence;
        public FoodTruckCollider TruckCollider;

        public FoodTruckParking CurrentParkingSpot { get; private set; }

        public UnityEvent OnStartDriving;
        public UnityEvent OnStopDriving;
        public event Action EnteredParkingSpot;
        public event Action LeftParkingSpot;

        public event Action<TruckState> EnteredState;
        public event Action StoppedServing;
        public bool IsServing
        {
            get => currentState == TruckState.Serving && CurrentParkingSpot != null;
        }
        [ShowInInspector, ReadOnly]
        private FoodTruckParking overlappedParkingSpot;
        void OnValidate()
        {
            if (Rigidbody == null)
            {
                Rigidbody = GetComponent<Rigidbody>();
            }

        }
        void Start()
        {
            TruckCollider.EnteredParking += OnEnteredParking;
            TruckCollider.ExitedParking += OnExitedParking;

            Seat.PlayerInSeatSeconaryInteraction += TryServe;

        }

        void OnDestroy()
        {
            TruckCollider.EnteredParking -= OnEnteredParking;
            TruckCollider.ExitedParking -= OnExitedParking;

        }


        void OnEnteredParking(FoodTruckParking parking)
        {
            overlappedParkingSpot = parking;
            EnteredParkingSpot?.Invoke();

        }
        void OnExitedParking(FoodTruckParking parking)
        {
            overlappedParkingSpot = null;
            if (CurrentParkingSpot != null)
            {
                CurrentParkingSpot = null;
            }
            LeftParkingSpot?.Invoke();

        }


        public TruckState CurrentState
        {
            get => currentState;
            set
            {
                switch (value)
                {
                    case TruckState.Stopped:
                        OnStopDriving?.Invoke();
                        break;
                    case TruckState.Driving:
                        OnStartDriving?.Invoke();
                        break;
                    case TruckState.Serving:
                        break;

                }
                if (currentState == TruckState.Serving && value != TruckState.Serving)
                {
                    StopServing();

                }
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
            Rigidbody.MovePosition(overlappedParkingSpot.transform.position);
            Rigidbody.MoveRotation(overlappedParkingSpot.transform.rotation);
            Rigidbody.linearVelocity = Vector3.zero;
            // Seat.PlayerInSeat.transform.SetParent(null);



            CurrentParkingSpot = overlappedParkingSpot;
            CurrentState = TruckState.Serving;
            return;
        }
        public void StopServing()
        {
            StoppedServing?.Invoke();
            OrderManager.I.RemoveProposedOrder();
            OrderManager.I.RemoveAllActiveOrders();

        }

        void StartDriving()
        {
            CurrentState = TruckState.Driving;
        }
        void StopDriving()
        {
            if (CurrentState == TruckState.Driving)
            {
                CurrentState = TruckState.Stopped;

            }
        }

        // public
        public float GetTruckSpeed()
        {
            return Rigidbody.linearVelocity.magnitude;
        }
        public bool IsMoving()
        {
            if (GetTruckSpeed() > .1f) return true;
            return false;
        }

#if UNITY_EDITOR
        GUIStyle style = new();
        void OnDrawGizmos()
        {
            style.normal.textColor = Color.blue;
            style.fontSize = 24;

            string message = "Truck Speed: " + GetTruckSpeed() + "\n";
            message += "Truck State: " + CurrentState.ToString() + "\n";
            if (overlappedParkingSpot != null)
            {
                message += "In parking spot\n";

            }
            Handles.Label(transform.position, message, style);

        }

        public Vector3 GetVelocity()
        {
            return Rigidbody.linearVelocity;
        }
#endif

    }
}