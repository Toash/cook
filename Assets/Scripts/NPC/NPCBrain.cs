using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// State machine for NPCs. <br/>
/// States should exist as children of this.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NPC))]
public class NPCBrain : MonoBehaviour
{

    public NPCState InitialState;
    [ReadOnly]
    public NPCState CurrentState;


    /// <summary>
    /// Invoked when the NPC becomes first in line.
    /// </summary>
    public event Action<NPCBrain> BecameFirstInLine;
    public NavMeshAgent Agent;
    /// <summary>
    ///  the current line that the npc is in.
    /// </summary>
    [ReadOnly]
    public OrderLine CurrentOrderLine;

    /// <summary>
    /// The current order that is associated with the NPC.
    /// </summary>
    [ReadOnly]
    public int CurrentOrderID;

    [ShowInInspector, ReadOnly]
    public Dictionary<string, NPCState> RegisteredStates = new();


    public NPC NPC { get; private set; }


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
            }
        }
    }
    void Start()
    {
        CurrentState = InitialState;
        CurrentState.OnEnter(this);
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



    public void SetCurrentOrderLine(OrderLine loc)
    {
        this.CurrentOrderLine = loc;
    }
    public OrderLine GetCurrentOrderLine()
    {
        return this.CurrentOrderLine;
    }
    /// <summary>
    /// Pass this as callbakc when subscribing to an OrderLocation line.
    /// </summary>
    /// <param name="brain"></param>
    public void OnFirstInLineChanged(NPCBrain brain)
    {
        if (brain != this) return;
        Debug.Log("[NPCBrain]: Became first in line");
        BecameFirstInLine?.Invoke(brain);
    }


#if UNITY_EDITOR
    GUIStyle style = new();
    void OnDrawGizmos()
    {
        style.normal.textColor = Color.blue;
        style.fontStyle = FontStyle.Bold;
        if (CurrentState != null)
        {
            Handles.Label(transform.position, "Current State: " + CurrentState.StateName, style);
        }

    }
#endif
}