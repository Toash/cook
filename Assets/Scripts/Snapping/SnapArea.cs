using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Represesnts a snap the snapper that other snappers can snap to.  <br/>
/// Snapping is done physically (transform hiearchy), as well as through bookkeeping.  <br/>
/// 
/// Each snapper can have multiple snap areas.
/// 
/// When placing a snapper, nearest snap areas get checked. Both snap areas get together.
/// </summary>
[RequireComponent(typeof(Collider))] // for snapping
public class SnapArea : MonoBehaviour
{
    [Tooltip("Whether or not only one snapper can be snapped to this snap area.")]
    public bool OnlyOneSnap = true;
    public bool SnapToCenter = true; // if true, the snapper will snap to the center of the snap area.  if false, it will snap to the exact position of the snap area.
    public Snapper ParentSnapper;
    // public SnapType AcceptingTypes; // the types that can snap to this.
    // public List<SnapType> AcceptingTypes; // the types that can snap to this.

    public List<Snapper> OccupiedSnappers;

    private Collider col;
    void OnValidate()
    {
        if (gameObject.layer != LayerMask.NameToLayer("Snapping"))
        {
            gameObject.layer = LayerMask.NameToLayer("Snapping");
        }

        if (col == null) col = GetComponent<Collider>();
        col.isTrigger = true;
    }
    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Snapping");
    }

    public bool TryAddSnapper(Snapper snapper)
    {
        if (OnlyOneSnap && OccupiedSnappers.Count > 0) return false;

        OccupiedSnappers.Add(snapper);
        void OnDetached()
        {
            OccupiedSnappers.Remove(snapper);
            snapper.Detached -= OnDetached;
        }
        snapper.Detached += OnDetached;
        return true;

    }


    /// <summary>
    /// Gets the location that should be snapped to.
    /// </summary>
    /// <param name="placementInfo"></param>
    /// <returns></returns>
    public Vector3 GetSnapPoint(PlacementInfo placementInfo)
    {
        if (SnapToCenter)
        {
            return transform.position;
        }
        else
        {
            return placementInfo.SnapRaycastHit.point;
        }
    }

    /// <summary>
    /// Gets the rotation that should be snapped to.
    /// </summary>
    /// <param name="placementInfo"></param>
    /// <returns></returns>
    public Quaternion GetSnapRotation(PlacementInfo placementInfo)
    {
        if (SnapToCenter)
        {
            return transform.rotation;
        }
        else
        {
            return Quaternion.identity;
        }
    }

#if UNITY_EDITOR

    GUIStyle style = new();
    void OnDrawGizmosSelected()
    {
        style.normal.textColor = Color.orange;
        Gizmos.color = Color.orange;
        string message = "";
        message += "Snap Area\n";
        Handles.Label(transform.position, message, style);

        if (ParentSnapper == null)
        {
            style.normal.textColor = Color.red;
            message += "Parent Snapper is not set!";
            Handles.Label(transform.position, message, style);
        }
    }
#endif
}
