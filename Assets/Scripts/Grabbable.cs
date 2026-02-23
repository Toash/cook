
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Grabbable : MonoBehaviour, IInteractable
{

    public GrabSettings GrabSettings;

    public event Action<InteractionContext> OnPrimaryInteract;
    public event Action<InteractionContext> OnSecondaryInteract;

    public Rigidbody Rb { get; private set; }

    private ConfigurableJoint joint;

    void Awake()
    {
        Rb = GetComponent<Rigidbody>();
    }
    public void Interact(InteractionContext context)
    {
        OnPrimaryInteract.Invoke(context);
        context.Grabber.TryGrab(this);
    }
    public void SecondaryInteract(InteractionContext context)
    {
        OnSecondaryInteract.Invoke(context);
    }

    // public void Drop()
    // {
    //     Destroy(joint);
    //     joint = null;

    // }

    public Transform GetTransform()
    {
        return transform;
    }

}