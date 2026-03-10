using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// An item that can be held by the player.
/// </summary>
public class Holdable : InteractableBase
{

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
        // context.Player.ItemHolder.TryHold(GetHoldableRoot());
        if (context.Type == InteractType.Primary)
        {
            context.Player.ItemHolder.TryHold(this);
        }

        beingHeld = true;
    }


    public void SetNotHolding()
    {
        beingHeld = false;
    }

    /// <summary>
    /// Combines linked visuals into one gameobject
    /// </summary>
    /// <param name="visualRoot"></param>
    public GameObject GetParentChildLinkedVisualRootClone()
    {
        var holdables = GetHoldableRoot().GetComponentsInChildren<Holdable>();
        // GameObject visualRoot = Instantiate(GetHoldableRoot().VisualRoot);

        int i = 0;
        GameObject parent = null;
        foreach (Holdable holdable in holdables)
        {
            GameObject holdableVisualRoot = Instantiate(holdable.VisualRoot, holdable.VisualRoot.transform.position, holdable.VisualRoot.transform.rotation);
            holdableVisualRoot.layer = LayerMask.NameToLayer("Overlay");
            foreach (var obj in holdableVisualRoot.GetComponentsInChildren<Transform>())
            {
                obj.gameObject.layer = LayerMask.NameToLayer("Overlay");
            }
            if (i == 0)
            {
                // parent
                parent = holdableVisualRoot;

            }
            holdableVisualRoot.transform.SetParent(parent.transform, true);
            i++;

        }
        return parent;
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
