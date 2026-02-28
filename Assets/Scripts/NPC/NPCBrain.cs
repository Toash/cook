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
public class NPCBrain : MonoBehaviour
{

    public NPCState CurrentState;


    /// <summary>
    /// Invoked when the NPC becomes first in line.
    /// </summary>
    public event Action<NPCBrain> BecameFirstInLine;
    /// <summary>
    ///  the current order location that the npc is at.
    /// </summary>
    private OrderLocation currentOrderLocation;

    public Dictionary<string, NPCState> RegisteredStates = new();

    public NavMeshAgent Agent;

    void OnValidate()
    {
        if (Agent == null)
        {
            Agent = GetComponent<NavMeshAgent>();
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
            CurrentState.OnExit(this);
            CurrentState = state;
            state.OnEnter(this);
        }
        else
        {
            Debug.LogError("[NPCBrain]: could not find state with name " + stateName);
        }
    }



    public void SetCurrentOrderLocation(OrderLocation loc)
    {
        this.currentOrderLocation = loc;
    }
    public OrderLocation GetCurrentOrderLocation()
    {
        return this.currentOrderLocation;
    }
    public void EnterLine()
    {

        currentOrderLocation.AddToLine(this);
        currentOrderLocation.NowFirstInLine += OnFirstInLineChanged;

    }
    public void ExitLine()
    {
        currentOrderLocation.RemoveFromLine();
        currentOrderLocation.NowFirstInLine -= OnFirstInLineChanged;
    }
    /// <summary>
    /// Pass this as callbakc when subscribing to an OrderLocation line.
    /// </summary>
    /// <param name="brain"></param>
    void OnFirstInLineChanged(NPCBrain brain)
    {
        if (brain != this) return;
        Debug.Log("[NPCBrain]: Became first in line");
        BecameFirstInLine?.Invoke(this);
    }


#if UNITY_EDITOR
    GUIStyle style = new();
    void OnDrawGizmos()
    {
        style.normal.textColor = Color.blue;
        style.fontStyle = FontStyle.Bold;
        Handles.Label(transform.position, "Current State: " + CurrentState.StateName, style);

    }
#endif
}