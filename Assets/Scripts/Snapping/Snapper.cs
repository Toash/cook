
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
    // /// <summary>
    // /// Empty list means all
    // /// </summary>
    // public List<SnapType> AcceptingSnapTypes = new List<SnapType>();

    [Sirenix.OdinInspector.ReadOnly]
    public List<SnapConnection> ParentSnapConnections = new List<SnapConnection>();
    [Sirenix.OdinInspector.ReadOnly]
    public List<SnapConnection> ChildSnapConnections = new List<SnapConnection>();


    public event Action Detached;
    public event Action<Snapper> OnChildSnapped;
    public event Action<Snapper> OnChildDetached;
    void OnDestroy()
    {
        Detached?.Invoke();

    }

    /// <summary>
    /// checks if this snapper can snap to its potential parent snap area.
    /// </summary>
    /// <param name="otherArea"></param>
    /// <returns></returns>
    public bool CanSnapToSnapArea(SnapArea otherArea)
    {
        // if other is null, just ignore joint type checks.
        if (otherArea != null)
        {
            if (!JointTypesCanSnap(otherArea)) return false;

            // already is occupied and only allows one snapper. 
            if (otherArea.OccupiedSnappers.Count > 0 && otherArea.OnlyOneSnap) return false;
        }
        return true;

    }

    bool JointTypesCanSnap(SnapArea other)
    {

        if (other.AcceptingSnapTypes.Count == 0) return true;
        if (!other.AcceptingSnapTypes.Contains(this.SnapType)) return false;



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
    public bool TrySnapToArea(PlacementInfo placementRaycastInfo, Snapper parentSnapper, SnapArea parentSnapArea)
    {
        if (!CanSnapToSnapArea(parentSnapArea))
        {
            Debug.Log("Cannot snap " + gameObject.name + " to " + parentSnapper.gameObject.name + " due to incompatible snap types.");
            return false;
        }


        // create snap connection
        SnapConnection thisConnection = new SnapConnection(parentSnapper, this);
        ParentSnapConnections.Add(thisConnection);
        parentSnapper.ChildSnapConnections.Add(thisConnection);


        parentSnapper.ChildSnapped(this);


        // occupy snap area
        // snapArea.OccupiedSnappers.Add(this);
        if (!parentSnapArea.TryAddSnapper(this))
        {
            return false;
        }

        //disable rigidbody if it exists
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
        }



        // actually place
        transform.position = parentSnapArea.GetSnapPoint(placementRaycastInfo);
        transform.rotation = parentSnapArea.GetSnapRotation(placementRaycastInfo);
        transform.SetParent(parentSnapper.transform, worldPositionStays: true);



        return true;
    }
    public bool TryGetPreviewPose(PlacementInfo info, out Vector3 pos, out Quaternion rot)
    {
        pos = default;
        rot = default;

        if (!TryGetBestSnapTarget(info, out SnapArea snapArea, out RaycastHit hit))
            return false;

        pos = snapArea.GetSnapPoint(info);
        rot = snapArea.GetSnapRotation(info);
        return true;
    }

    public bool TryPlaceFromPlacementInfo(PlacementInfo info)
    {
        if (!TryGetBestSnapTarget(info, out SnapArea snapArea, out RaycastHit hit))
            return false;

        Snapper otherSnapper = snapArea.ParentSnapper;
        return TrySnapToArea(info, otherSnapper, snapArea);
    }

    private bool TryGetBestSnapTarget(PlacementInfo info, out SnapArea snapArea, out RaycastHit validHit)
    {
        return info.TryGetFirstValidSnapArea(this, out snapArea, out validHit);
    }


    /// <summary>
    /// Called when this is detached from a snapper.
    /// </summary>
    public void DetachFromParent()
    {
        // detach from all parent snap connections
        foreach (SnapConnection connection in ParentSnapConnections)
        {
            connection.Parent.ChildSnapConnections.Remove(connection);
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

        if (includeSelf)
            visited.Add(this);

        stack.Push(this);

        while (stack.Count > 0)
        {
            Snapper current = stack.Pop();

            foreach (SnapConnection connection in current.ChildSnapConnections)
            {
                Snapper childSnapper = connection.Child;
                if (childSnapper == null) continue;

                if (visited.Add(childSnapper))
                    stack.Push(childSnapper);
            }
        }

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


        if (CanSnapToSnapArea(null))
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