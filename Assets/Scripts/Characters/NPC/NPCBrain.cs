using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NPC))]
[RequireComponent(typeof(Rigidbody))]
public class NPCBrain : MonoBehaviour
{
    [Header("States")]
    public string InitialStateString;
    public string DefaultStateString = "NPCDailySchedule";
    public bool HasEatenToday;

    [ReadOnly] public NPCState CurrentState;
    [ShowInInspector, ReadOnly]
    public Dictionary<string, NPCState> RegisteredStates = new();

    [Header("References")]
    public NPC NPC { get; private set; }
    public NavMeshAgent Agent { get; private set; }

    [Header("Order Context")]
    public OrderStation CurrentOrderStation { get; private set; }
    [ReadOnly] public int CurrentOrderID;
    [ReadOnly] public float TimeSinceLastAte;

    public event Action LineChanged;

    private Rigidbody rb;
    public SphereCollider TriggerCol;

    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        NPC = GetComponent<NPC>();

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        if (TriggerCol != null)
        {
            TriggerCol.isTrigger = true;
            TriggerCol.gameObject.layer = LayerMask.NameToLayer("NPCTrigger");
        }

        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<NPCState>(out var state))
            {
                RegisteredStates.Add(state.Name, state);
                state.Brain = this;
                state.NPC = NPC;
            }
        }
    }

    void Start()
    {
        ChangeState(InitialStateString);
    }

    void Update()
    {
        CurrentState?.OnUpdate(this);
    }

    void FixedUpdate()
    {
        CurrentState?.OnFixedUpdate(this);
    }

    public void ChangeState(string stateName)
    {
        if (!RegisteredStates.TryGetValue(stateName, out NPCState newState))
        {
            Debug.LogError("[NPCBrain]: could not find state " + stateName);
            return;
        }

        if (CurrentState != null)
            CurrentState.OnExit(this);

        CurrentState = newState;
        CurrentState.OnEnter(this);
    }

    public void SetOrderStation(OrderStation station)
    {
        CurrentOrderStation = station;
    }

    public void ClearOrderStation()
    {
        CurrentOrderStation = null;
    }

    public void OnLineChanged(NPCBrain brain)
    {
        if (brain != this) return;
        LineChanged?.Invoke();
    }

#if UNITY_EDITOR
    GUIStyle style = new();
    void OnDrawGizmos()
    {
        style.normal.textColor = Color.blue;
        style.fontStyle = FontStyle.Bold;

        string message = "";
        if (CurrentState != null)
            message += "CurrentState: " + CurrentState.Name + "\n";

        if (CurrentOrderStation != null)
            message += "At station: " + CurrentOrderStation.name;

        Handles.Label(transform.position, message, style);
    }
#endif
}