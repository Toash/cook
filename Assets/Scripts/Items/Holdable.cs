using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// An item that can be held by the player.
/// </summary>
public class Holdable : InteractableBase
{

    [Tooltip("Root object for whatever visually represents this item. Ex. used for previews when placing said item if its held.")]
    public GameObject VisualRoot;
    public event Action<InteractionContext> OnHeld;
    public event Action<InteractionContext> OnSecondaryInteract;


    private ConfigurableJoint joint;
    private bool beingHeld = false;


    /// <summary>
    /// Returns the root of holdables.
    /// 
    /// Stops at the player hand if the player is holding.
    /// </summary>
    /// <returns></returns>
    public Holdable GetHoldableRoot()
    {

        Transform t = transform;

        while (t.parent != null)
        {
            if (t.parent.GetComponent<PlayerHand>() != null)
            {
                break;
            }

            t = t.parent;
        }
        return t.GetComponentInChildren<Holdable>();
    }

    public bool IsBeingHeld()
    {
        return beingHeld;
    }
    public override void Interact(InteractionContext context)
    {
        OnHeld?.Invoke(context);
        context.Player.ItemHolder.TryHold(GetHoldableRoot());

        beingHeld = true;
    }
    public void SecondaryInteract(InteractionContext context)
    {
        OnSecondaryInteract.Invoke(context);
    }


    public void SetNotHolding()
    {
        beingHeld = false;
    }

    /// <summary>
    /// Combines linked visuals into one gameobject
    /// </summary>
    /// <param name="visualRoot"></param>
    public GameObject GetVisualRoot()
    {
        var holdables = GetHoldableRoot().GetComponentsInChildren<Holdable>();
        GameObject visualRoot = GetHoldableRoot().VisualRoot;

        foreach (var holdable in holdables)
        {
            if (holdable == this) continue;

            holdable.VisualRoot.transform.SetParent(visualRoot.transform, true);

            // transform relative to the root.
            // var relPos = visualRoot.transform.InverseTransformPoint(holdable.VisualRoot.transform.position);
            // var relRot = Quaternion.Inverse(visualRoot.transform.rotation) * holdable.VisualRoot.transform.rotation;
            // clone.transform.localPosition = relPos;
            // clone.transform.localRotation = relRot;
        }
        return visualRoot;
    }



#if UNITY_EDITOR
    public float GizmoSize = .3f;
    void OnDrawGizmos()
    {
        if (beingHeld)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        if (VisualRoot == null)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            Handles.Label(transform.position, "VisualRoot not set!", style);
        }

        Gizmos.DrawWireCube(transform.position, Vector3.one * GizmoSize);

    }

#endif
}
