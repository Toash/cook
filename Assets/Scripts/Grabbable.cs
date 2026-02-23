
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Grabbable : MonoBehaviour, IInteractable
{

    public GrabSettings GrabSettings;

    public event Action<InteractionContext> OnPrimaryInteract;
    public event Action<InteractionContext> OnSecondaryInteract;

    private ConfigurableJoint joint;

    public void Interact(InteractionContext context)
    {
        OnPrimaryInteract.Invoke(context);
        context.Grabber.TryGrab(this);
    }
    public void SecondaryInteract(InteractionContext context)
    {
        OnSecondaryInteract.Invoke(context);
    }

    public void GetGrabbed(PlayerGrabber grabber)
    {
        joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;
        joint.connectedAnchor = Vector3.zero;

        joint.xMotion = ConfigurableJointMotion.Free;
        joint.yMotion = ConfigurableJointMotion.Free;
        joint.zMotion = ConfigurableJointMotion.Free;

        joint.angularXMotion = ConfigurableJointMotion.Locked;
        joint.angularYMotion = ConfigurableJointMotion.Locked;
        joint.angularZMotion = ConfigurableJointMotion.Locked;

        JointDrive xDrive = joint.xDrive;
        xDrive.positionSpring = GrabSettings.PositionSpring;
        xDrive.positionDamper = GrabSettings.PositionDamper;
        joint.xDrive = xDrive;

        JointDrive yDrive = joint.yDrive;
        yDrive.positionSpring = GrabSettings.PositionSpring;
        yDrive.positionDamper = GrabSettings.PositionDamper;
        joint.yDrive = yDrive;

        JointDrive zDrive = joint.zDrive;
        zDrive.positionSpring = GrabSettings.PositionSpring;
        zDrive.positionDamper = GrabSettings.PositionDamper;
        joint.zDrive = zDrive;


        // set to grab handle
        joint.connectedBody = grabber.HandleRb;

    }
    public void Drop()
    {
        Destroy(joint);
        joint = null;

    }

    public Transform GetTransform()
    {
        return transform;
    }

}