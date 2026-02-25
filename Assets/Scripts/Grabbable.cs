
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Grabbable : InteractableBase
{

    public GrabSettings GrabSettings;

    public event Action<InteractionContext> OnGrab;
    public event Action<InteractionContext> OnDrop;
    public event Action<InteractionContext> OnSecondaryInteract;

    public Rigidbody Rb { get; private set; }

    private ConfigurableJoint joint;
    private bool beingHeld = false;


    public bool IsBeingHeld()
    {
        return beingHeld;
    }

    void Awake()
    {
        Rb = GetComponent<Rigidbody>();
    }
    public override void Interact(InteractionContext context)
    {
        OnGrab.Invoke(context);
        context.Grabber.TryGrab(this);

        beingHeld = true;
    }
    public void SecondaryInteract(InteractionContext context)
    {
        OnSecondaryInteract.Invoke(context);
    }

    public void Drop(InteractionContext context)
    {
        OnDrop.Invoke(context);
        beingHeld = false;
    }

    public Transform GetTransform()
    {
        return transform;
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
        Gizmos.DrawWireCube(transform.position, Vector3.one * GizmoSize);

    }

#endif

}