using System;
using System.Collections.Generic;
using IngameDebugConsole;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Assets.Scripts.Ingredient.MenuItem;

namespace Assets.Scripts.Vehicle
{
    // public enum TruckState
    // {
    //     Stopped,
    //     Driving,
    //     Serving
    // }
    public enum VehicleMotionState
    {
        Stopped,
        Driving
    }
    public enum ServiceState
    {
        Closed,
        Open
    }
    [RequireComponent(typeof(Rigidbody))] // used to check speed 
    public class FoodTruck : MonoBehaviour, IVelocityProvider
    {
        [Header("Orderstuff")]
        public List<TruckMenuItem> MenuItems = new List<TruckMenuItem>();
        public SingleOrderSubmissionArea OrderSubmissionArea;
        public Transform OrderSpot;
        [Header("Truck stuff")]
        public Animator CoverAnimator;
        public string OpenCoverAnimBool = "isOpen";
        public CarSeat Seat;
        public FoodTruckInfluence Influence;
        public FoodTruckCollider TruckCollider;

        public FoodTruckParking CurrentParkingSpot { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        public UnityEvent OnStartDriving;
        public UnityEvent OnStopDriving;
        public event Action EnteredParkingSpot;
        public event Action LeftParkingSpot;

        // public event Action<TruckState, TruckState> EnteredState;
        public event Action StoppedServing;
        // public bool IsServing
        // {
        //     get => currentState == TruckState.Serving && CurrentParkingSpot != null;
        // }
        [ShowInInspector, ReadOnly]
        private FoodTruckParking overlappedParkingSpot;

        public event Action<VehicleMotionState, VehicleMotionState> MotionStateChanged;
        public event Action<ServiceState, ServiceState> ServiceStateChanged;

        private VehicleMotionState motionState = VehicleMotionState.Stopped;
        public VehicleMotionState MotionState
        {
            get => motionState;
            private set
            {
                if (motionState == value) return;
                var previous = motionState;
                motionState = value;

                switch (value)
                {
                    case VehicleMotionState.Stopped:
                        OnStopDriving?.Invoke();
                        break;
                    case VehicleMotionState.Driving:
                        OnStartDriving?.Invoke();
                        break;
                }

                MotionStateChanged?.Invoke(previous, value);
            }
        }

        private ServiceState serviceState = ServiceState.Closed;
        public ServiceState ServiceState
        {
            get => serviceState;
            private set
            {
                if (serviceState == value) return;
                var previous = serviceState;
                serviceState = value;
                ServiceStateChanged?.Invoke(previous, value);
            }
        }

        public bool IsServing => ServiceState == ServiceState.Open;
        public bool IsParked => CurrentParkingSpot != null;
        void OnValidate()
        {
            if (Rigidbody == null)
            {
                Rigidbody = GetComponent<Rigidbody>();
            }

        }
        void Start()
        {
            ServiceStateChanged += OnServingStateChanged;

            TruckCollider.EnteredParking += OnEnteredParking;
            TruckCollider.ExitedParking += OnExitedParking;

            Seat.PlayerInSeatSeconaryInteraction += TryServe;

        }

        void OnDestroy()
        {
            ServiceStateChanged -= OnServingStateChanged;

            TruckCollider.EnteredParking -= OnEnteredParking;
            TruckCollider.ExitedParking -= OnExitedParking;

            Seat.PlayerInSeatSeconaryInteraction -= TryServe;

        }


        void OnEnteredParking(FoodTruckParking parking)
        {
            overlappedParkingSpot = parking;
            CurrentParkingSpot = parking;
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
        public bool CanOpenForService()
        {
            return MotionState == VehicleMotionState.Stopped && IsParked && !IsMoving();
        }

        public void OnServingStateChanged(ServiceState prev, ServiceState current)
        {
            if (CoverAnimator != null)
            {
                switch (current)
                {
                    case ServiceState.Open:
                        CoverAnimator.SetBool(OpenCoverAnimBool, true);
                        break;
                    case ServiceState.Closed:
                        CoverAnimator.SetBool(OpenCoverAnimBool, false);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets a random menu item from the trucks menu
        /// </summary>
        /// <returns></returns>
        public TruckMenuItem GetRandomMenuItem()
        {
            if (MenuItems.Count == 0)
            {
                Debug.LogWarning("[FoodTruck]: No menu items in truck!");
                return null;
            }
            return MenuItems[UnityEngine.Random.Range(0, MenuItems.Count)];
        }



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
            // if (CurrentState != TruckState.Serving)
            // {

            //     StopDriving();
            // }
            if (!IsServing)
            {
                StopDriving();
            }
        }



        public bool CanPark()
        {
            if (MotionState != VehicleMotionState.Driving) return false;
            if (overlappedParkingSpot == null) return false;
            if (IsMoving()) return false;
            return true;

        }

        /// <summary>
        /// Start accepting customers
        /// </summary>
        /// <returns></returns>
        [Button]
        public void TryServe()
        {
            if (overlappedParkingSpot == null)
            {
                Debug.Log("[FoodTruck]: Can't serve, no parking spot");
                return;
            }

            Rigidbody.MovePosition(overlappedParkingSpot.transform.position);
            Rigidbody.MoveRotation(overlappedParkingSpot.transform.rotation);
            Rigidbody.linearVelocity = Vector3.zero;

            CurrentParkingSpot = overlappedParkingSpot;
            MotionState = VehicleMotionState.Stopped;

            if (!CanOpenForService())
            {
                Debug.Log("[FoodTruck]: Can't open for service");
                return;
            }

            Debug.Log("[FoodTruck]: Serving");
            ServiceState = ServiceState.Open;
        }
        public void StopServing()
        {
            // StoppedServing?.Invoke();
            // OrderManager.I.RemoveProposedOrder();
            // OrderManager.I.RemoveAllActiveOrders();

            if (!IsServing) return;

            ServiceState = ServiceState.Closed;
            StoppedServing?.Invoke();
            OrderManager.I.RemoveProposedOrder();
            OrderManager.I.RemoveAllActiveOrders();

        }

        void StartDriving()
        {
            // CurrentState = TruckState.Driving;
            if (IsServing)
            {
                StopServing();
            }

            MotionState = VehicleMotionState.Driving;
        }
        void StopDriving()
        {
            // if (CurrentState == TruckState.Driving)
            // {
            //     CurrentState = TruckState.Stopped;

            // }
            if (MotionState == VehicleMotionState.Driving)
            {
                MotionState = VehicleMotionState.Stopped;
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
            message += "Motion State: " + MotionState + "\n";
            message += "Service State: " + ServiceState + "\n";
            if (overlappedParkingSpot != null)
            {
                message += "In parking spot\n";

            }
            Handles.Label(transform.position, message, style);

            if (OrderSpot != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(OrderSpot.position, .2f);
            }

        }

        public Vector3 GetVelocity()
        {
            return Rigidbody.linearVelocity;
        }
#endif

    }
}