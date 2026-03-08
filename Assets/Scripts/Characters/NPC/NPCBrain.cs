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
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NPC))]

[RequireComponent(typeof(Rigidbody))] // used for checking ontriggerenters

public class NPCBrain : MonoBehaviour
{

    [Header("States")]
    public NPCState InitialState;
    [ReadOnly]
    public NPCState CurrentState;

    [ShowInInspector, ReadOnly]
    public Dictionary<string, NPCState> RegisteredStates = new();

    [Header("References")]
    public NPC NPC { get; private set; }
    public NavMeshAgent Agent;

    // ------- FOOD TRUCK-----------------    

    [ReadOnly]
    public FoodTruck CurrentFoodTruck;
    //[ReadOnly]
    //public OrderLine CurrentOrderLine;
    [ReadOnly]
    public int CurrentOrderID;
    /// <summary>
    /// Invoked when the NPC becomes first in line.
    /// </summary>
    public event Action<NPCBrain> BecameFirstInLine;
    public event Action LineChanged;
    public event Action<FoodTruck> EnteredFoodTruck;

    private Rigidbody rb;
    public SphereCollider TriggerCol;

    void OnValidate()
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
    }

    void Awake()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<NPCState>(out var state))
            {
                Debug.Log("[NPCBrain]: Adding state " + state.StateName + " to RegisteredStates.");
                RegisteredStates.Add(state.StateName, state);
                state.Brain = this;
                state.NPC = NPC;
            }
        }
    }
    void Start()
    {
        CurrentState = InitialState;
        CurrentState.OnEnter(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered trigger: " + other.name);
        if (other.attachedRigidbody.TryGetComponent<FoodTruck>(out var truck))
        {
            //CurrentFoodTruck = truck;
            //ChangeState("NPCWaitInLine");
            EnteredFoodTruck?.Invoke(truck);
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
            CurrentState.OnExit(this);
            CurrentState = state;
            state.OnEnter(this);
        }
        else
        {
            Debug.LogError("[NPCBrain]: could not find state with name " + stateName);
        }
    }



    /// <summary>
    /// Pass this as callbakc when subscribing to an OrderLocation line.
    /// </summary>
    /// <param name="brain"></param>
    public void OnFirstInLineChanged(NPCBrain brain)
    {
        LineChanged?.Invoke();
        if (brain != this) return;
        Debug.Log("[NPCBrain]: Became first in line");
        BecameFirstInLine?.Invoke(brain);
    }


#if UNITY_EDITOR
    GUIStyle style = new();
    void OnDrawGizmosSelected()
    {
        style.normal.textColor = Color.blue;
        style.fontStyle = FontStyle.Bold;
        string message = "";
        if (InitialState != null)
        {
            message += "Initial State: " + InitialState.StateName + "\n";
        }
        if (CurrentState != null)
        {
            message += "CurrentState: " + CurrentState.StateName + "\n";
        }
        Handles.Label(transform.position, message, style);

        if (CurrentFoodTruck != null)
        {
            Handles.Label(transform.position + (Vector3.up * .3f), "In food truck", style);
        }

    }
#endif
}