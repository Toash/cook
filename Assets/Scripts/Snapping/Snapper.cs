
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;


/// <summary>
/// System for snapping objects together.  <br/>
/// Uses transform hiearchy for snapping
/// </summary>
[RequireComponent(typeof(Holdable))]
public class Snapper : MonoBehaviour
{



    [Tooltip("Determines what type of snappers can snap to others.")]
    [ShowInInspector]
    public SnapType SnapType { get; private set; }

    public List<SnapConnection> SnapConnections = new List<SnapConnection>();


    public event Action<SnapConnection> OnSnap;
    public event Action<SnapConnection> OnDetached;
    public bool CanSnap(Snapper other)
    {
        // if other is null, just ignore joint type checks.
        if (other != null)
        {
            if (!JointTypesCanSnap(other)) return false;
        }
        return true;

    }

    bool JointTypesCanSnap(Snapper other)
    {
        if (SnapType == SnapType.Receipt && other.SnapType == SnapType.Food)
        {
            return false;
        }
        if (SnapType == SnapType.Food && other.SnapType == SnapType.Receipt)
        {
            return false;
        }

        return true;
    }

    public void SetSnapType(SnapType type)
    {
        this.SnapType = type;
    }


    public void SnapToArea(PlacementRaycastInfo placementRaycastInfo, Snapper otherSnapper, SnapArea otherSnapArea)
    {
        if (!CanSnap(otherSnapper))
        {
            Debug.Log("Cannot snap " + gameObject.name + " to " + otherSnapper.gameObject.name + " due to incompatible snap types.");
            return;
        }

        // update snap connections 
        SnapConnection thisConnection = new SnapConnection(this, otherSnapper, otherSnapArea);
        SnapConnection otherConnection = new SnapConnection(otherSnapper, this, otherSnapArea);
        SnapConnections.Add(thisConnection);
        otherSnapper.SnapConnections.Add(otherConnection);

        _OnSnap(thisConnection);
        otherSnapper._OnSnap(otherConnection);


        // actually snap the object
        if (otherSnapArea.SnapToCenter)
        {
            transform.position = otherSnapArea.transform.position;
        }
        else
        {
            transform.position = placementRaycastInfo.SnapRaycastHit.point;
        }
        transform.rotation = otherSnapArea.transform.rotation;

        transform.SetParent(otherSnapper.transform);

    }



    // / <summary>
    // / Returns all of the snappers connected by snap collections recursively
    // / </summary>
    // / <returns></returns>
    public List<Snapper> GetSnapperGroup(bool includeSelf = true)
    {
        var visited = new HashSet<Snapper>();
        var stack = new Stack<Snapper>();

        visited.Add(this);
        stack.Push(this);

        while (stack.Count > 0)
        {
            Snapper current = stack.Pop();

            foreach (SnapConnection connection in current.SnapConnections)
            {
                Snapper otherSnapper = connection.Other;
                if (otherSnapper == null) continue;

                if (visited.Add(otherSnapper))
                    stack.Push(otherSnapper);
            }
        }

        if (!includeSelf) visited.Remove(this);
        return visited.ToList();
    }

    /// <summary>
    /// Called when a snapper snaps to another snapper. Called on both snappers
    /// </summary>
    /// <param name="other"></param>
    /// <param name="currentSnapConnection"></param>
    void _OnSnap(SnapConnection currentSnapConnection)
    {
        Debug.Log("[Snapper]: Calling OnSnap with connection  " + currentSnapConnection);
        OnSnap?.Invoke(currentSnapConnection);
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {


        if (CanSnap(null))
        {

            Gizmos.color = Color.green;
        }

        Gizmos.color = Color.blue;

        // visualize snap connections
        string message = "";

        message += "SnapType: " + SnapType.ToString() + "\n";

        // foreach (SnapConnection connection in SnapConnections)
        // {
        //     message += connection.ToString() + "\n";
        // }


        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.blue;
        style.fontStyle = FontStyle.Bold;
        Handles.Label(transform.position + (Vector3.up * 1), message, style);
    }
#endif


}