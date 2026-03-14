
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
    /// <summary>
    /// Empty list means all
    /// </summary>
    public List<SnapType> AcceptingSnapTypes = new List<SnapType>();

    [Sirenix.OdinInspector.ReadOnly]
    public List<SnapConnection> ParentSnapConnections = new List<SnapConnection>();


    public event Action Detached;
    public event Action<Snapper> OnChildSnapped;
    public event Action<Snapper> OnChildDetached;
    public bool CanSnap(SnapArea otherArea)
    {
        // if other is null, just ignore joint type checks.
        if (otherArea != null)
        {
            if (!JointTypesCanSnap(otherArea.ParentSnapper)) return false;

            // already is occupied and only allows one snapper. 
            if (otherArea.OccupiedSnappers.Count > 0 && otherArea.OnlyOneSnap) return false;
        }
        return true;

    }

    bool JointTypesCanSnap(Snapper other)
    {
        // if (SnapType == SnapType.Receipt && other.SnapType == SnapType.Food)
        // {
        //     return false;
        // }
        // if (SnapType == SnapType.Food && other.SnapType == SnapType.Receipt)
        // {
        //     return false;
        // }

        if (other.AcceptingSnapTypes.Count == 0) return true;
        if (!other.AcceptingSnapTypes.Contains(SnapType)) return false;



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
    /// <param name="otherSnapper"></param>
    /// <param name="otherSnapArea"></param>
    public bool TrySnapToArea(PlacementInfo placementRaycastInfo, Snapper otherSnapper, SnapArea otherSnapArea)
    {
        if (!CanSnap(otherSnapArea))
        {
            Debug.Log("Cannot snap " + gameObject.name + " to " + otherSnapper.gameObject.name + " due to incompatible snap types.");
            return false;
        }


        // create snap connection
        SnapConnection thisConnection = new SnapConnection(otherSnapper);
        ParentSnapConnections.Add(thisConnection);
        otherSnapper.ChildSnapped(this);


        // occupy snap area
        // snapArea.OccupiedSnappers.Add(this);
        if (!otherSnapArea.TryAddSnapper(this))
        {
            return false;
        }

        //disable rigidbody if it exists
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
        }



        // actually place
        transform.position = otherSnapArea.GetSnapPoint(placementRaycastInfo);
        transform.rotation = otherSnapArea.GetSnapRotation(placementRaycastInfo);
        transform.SetParent(otherSnapper.transform, worldPositionStays: true);

        return true;
    }


    /// <summary>
    /// Called when this is detached from a snapper.
    /// </summary>
    public void Detach()
    {
        // detach from all parent snap connections
        foreach (var connection in ParentSnapConnections)
        {
            connection.Parent.ChildDetached(this);

        }
        ParentSnapConnections.Clear();




        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
        }
        transform.SetParent(null, true);

        Detached?.Invoke();
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

            foreach (SnapConnection connection in current.ParentSnapConnections)
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