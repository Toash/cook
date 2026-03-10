
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
/// Uses transform hiearchy for snapping <br/>
/// Snapping connections are one way (child to parent).  <br/>
/// </summary>
public class Snapper : MonoBehaviour
{



    [Tooltip("Determines what type of snappers can snap to others.")]
    [ShowInInspector]
    public SnapType SnapType { get; private set; }

    public List<SnapConnection> SnapConnections = new List<SnapConnection>();


    public event Action<Snapper> OnChildSnapped;
    public event Action<Snapper> OnChildDetached;
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


    /// <summary>
    /// Snap this snapper to another snapper from their snap area.
    /// </summary>
    /// <param name="placementRaycastInfo"></param>
    /// <param name="parentSnapper"></param>
    /// <param name="parentSnapArea"></param>
    public void SnapToArea(PlacementInfo placementRaycastInfo, Snapper parentSnapper, SnapArea parentSnapArea)
    {
        if (!CanSnap(parentSnapper))
        {
            Debug.Log("Cannot snap " + gameObject.name + " to " + parentSnapper.gameObject.name + " due to incompatible snap types.");
            return;
        }

        // create snap connection
        SnapConnection thisConnection = new SnapConnection(parentSnapper);
        SnapConnections.Add(thisConnection);
        parentSnapper.ChildSnapped(this);


        //disable rigidbody if it exists
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
        }



        // actually place
        transform.position = parentSnapArea.GetSnapPoint(placementRaycastInfo);
        transform.rotation = parentSnapArea.GetSnapRotation(placementRaycastInfo);
        transform.SetParent(parentSnapper.transform, worldPositionStays: true);
    }


    /// <summary>
    /// Called when this is detached from a snapper.
    /// </summary>
    public void Detach()
    {
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
        }

    }


    public List<Snapper> GetSnapperChildrenRecursive(bool includeSelf = true)
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
                Snapper otherSnapper = connection.Parent;
                if (otherSnapper == null) continue;

                if (visited.Add(otherSnapper))
                    stack.Push(otherSnapper);
            }
        }

        if (!includeSelf) visited.Remove(this);
        return visited.ToList();
    }

    void ChildSnapped(Snapper child)
    {
        OnChildSnapped?.Invoke(child);
    }

    public void ChildDetached(Snapper child)
    {
        OnChildDetached?.Invoke(child);
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