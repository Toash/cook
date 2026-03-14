using Assets.Scripts.Characters.NPC;
using Assets.Scripts.Vehicle;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// State machine for NPCs. <br/>
/// States should exist as children of this.
/// States are referred to by just a string.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NPC))]

[RequireComponent(typeof(Rigidbody))] // used for checking ontriggerenters

public class NPCBrain : MonoBehaviour
{

    [Header("States")]
    public string InitialStateString;
    public string DefaultStateString = "NPCDailySchedule";
    [ReadOnly]
    public NPCState CurrentState;

    [ShowInInspector, ReadOnly]
    public Dictionary<string, NPCState> RegisteredStates = new();

    [Header("References")]
    public NPC NPC { get; private set; }
    public NavMeshAgent Agent { get; private set; }

    // ------- FOOD TRUCK-----------------    

    [ShowInInspector, ReadOnly]
    public FoodTruck CurrentFoodTruck;
    // {
    //     get
    //     {
    //         if (_currentFoodTruck != null)
    //         {
    //             return _currentFoodTruck;
    //         }
    //         else
    //         {
    //             Debug.LogError("[NPCBrain]: current food truck is null!");
    //             if (CurrentState.Name != DefaultStateString)
    //             {
    //                 ChangeState(DefaultStateString);

    //             }
    //             return null;
    //         }
    //     }
    //     set => _currentFoodTruck = value;

    // }
    // private FoodTruck _currentFoodTruck;
    //[ReadOnly]
    //public OrderLine CurrentOrderLine;
    [ReadOnly]
    public int CurrentOrderID;
    [ReadOnly]
    public float TimeSinceLastAte;
    /// <summary>
    /// Invoked when the NPC becomes first in line.
    /// </summary>
    public event Action LineChanged;
    public event Action<FoodTruck> EnteredFoodTruck;
    public event Action<FoodTruck> ExitedFoodTruck;

    private Rigidbody rb;
    public SphereCollider TriggerCol;


    void Awake()
    {
        if (Agent == null)
        {
            Agent = GetComponent<NavMeshAgent>();
        }
        if (NPC == null)
        {
            NPC = GetComponent<NPC>();
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        if (TriggerCol != null)
        {
            TriggerCol.isTrigger = true;
            TriggerCol.gameObject.layer = LayerMask.NameToLayer("NPCTrigger");
        }
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<NPCState>(out var state))
            {
                Debug.Log("[NPCBrain]: Adding state " + state.Name + " to RegisteredStates.");
                RegisteredStates.Add(state.Name, state);
                state.Brain = this;
                state.NPC = NPC;
            }
        }
    }
    void Start()
    {
        // CurrentState.OnEnter(this);
        ChangeState(InitialStateString);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered trigger: " + other.name);

    }
    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<FoodTruckInfluence>(out var influence))
        {
            if (influence.FoodTruck.IsServing && CurrentFoodTruck == null)
            {
                EnterTruck(influence.FoodTruck);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<FoodTruckInfluence>(out var influence))
        {
            if (CurrentFoodTruck != null)
            {
                LeaveCurrentTruck();
            }
        }
    }
    void Update()
    {
        CurrentState.OnUpdate(this);
    }
    void FixedUpdate()
    {
        CurrentState.OnFixedUpdate(this);
    }
    public void ChangeState(string stateName)
    {
        if (RegisteredStates.TryGetValue(stateName, out NPCState state))
        {
            if (CurrentState != null)
            {
                Debug.Log("[NPCBrain]: Changing state from " + CurrentState.Name + "to " + state.Name);
                CurrentState.OnExit(this);
                CurrentState = state;
                state.OnEnter(this);

            }
            else
            {
                Debug.Log("[NPCBrain]: Changing state to " + state.Name);
                CurrentState = state;
                state.OnEnter(this);

            }


        }
        else
        {
            Debug.LogError("[NPCBrain]: could not find state with name " + stateName);
        }
    }



    /// <summary>
    /// Pass this as callback when subscribing to an OrderLocation line.
    /// </summary>
    /// <param name="brain"></param>
    public void OnLineChanged(NPCBrain brain)
    {
        LineChanged?.Invoke();
        if (brain != this) return;
        Debug.Log("[NPCBrain]: Became first in line");
    }

    void EnterTruck(FoodTruck truck)
    {
        Debug.Log("[NPCBrain]: Entered food truck");
        CurrentFoodTruck = truck;
        truck.StoppedServing += LeaveCurrentTruck;
        truck.LeftParkingSpot += LeaveCurrentTruck;
        EnteredFoodTruck?.Invoke(truck);

    }
    void LeaveCurrentTruck()
    {
        if (CurrentFoodTruck == null) Debug.LogError("[NPCBrain]: No current truck");

        Debug.Log("[NPCBrain]: Left food truck");
        ExitedFoodTruck?.Invoke(CurrentFoodTruck);

        CurrentFoodTruck.StoppedServing -= LeaveCurrentTruck;
        CurrentFoodTruck.LeftParkingSpot -= LeaveCurrentTruck;

        ChangeState(DefaultStateString);

        CurrentFoodTruck = null;// do afterwards so it is not null when states try to access it.
    }


#if UNITY_EDITOR
    GUIStyle style = new();
    void OnDrawGizmos()
    {
        style.normal.textColor = Color.blue;
        style.fontStyle = FontStyle.Bold;
        string message = "";
        // if (InitialStateString != null)
        // {
        //     message += "Initial State: " + InitialStateString + "\n";
        // }
        if (CurrentState != null)
        {
            message += "CurrentState: " + CurrentState.Name + "\n";
        }
        Handles.Label(transform.position, message, style);

        if (CurrentFoodTruck != null)
        {
            Handles.Label(transform.position + (Vector3.up * .3f), "In food truck", style);
        }

    }
#endif
}