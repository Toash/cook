using System;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

/// <summary>
/// Deprecated, player uses ItemHolder to pick things up now (non physics)
/// </summary>
public class PlayerPhysicsGrabber : MonoBehaviour
{
    public float MinCamDist = 1;
    public float MaxCamDist = 3;
    public float ScrollSensitivity = .1f;
    public float RotateSensitivity = .05f;

    [Header("References")]
    public GrabberHandle GrabberHandle;
    public PlayerController playerController;
    public GameObject GrabLight;

    [Header("Actions")]
    public InputActionReference Rotate;
    public InputActionReference Mouse;
    public InputActionReference Scroll;


    private float handlePitch = 0;
    private float handleYaw = 0;

    // Quaternion targetRot;
    // bool wasRotating;

    public Transform CamRoot;

    public bool isHolding
    {
        get
        {
            return held != null;
        }
    }

    PhysicsGrabbable held;
    private float handleDistanceFromCamera = 2;

    private ConfigurableJoint handleConfigurableJoint;

    void OnEnable()
    {
        GrabberHandle.DroppedByPhysics += DroppedByPhysics;
    }
    void OnDisable()
    {
        GrabberHandle.DroppedByPhysics -= DroppedByPhysics;
    }


    void Update()
    {
        // GrabLight.SetActive(isHolding);
        HandleHandleRotation();
        float scrollDelta = Scroll.action.ReadValue<float>();
        float delta = scrollDelta * ScrollSensitivity;
        handleDistanceFromCamera = Mathf.Clamp(handleDistanceFromCamera += delta, MinCamDist, MaxCamDist);


        GrabberHandle.transform.position = CamRoot.transform.position + (CamRoot.forward * handleDistanceFromCamera);
    }
    public void OnInteractAndGrabbing(InteractionContext context)
    {
        if (context.Type == InteractType.Primary)
        {
            Drop();
        }
        else if (context.Type == InteractType.Secondary)
        {
            SecondaryInteract(context);

        }

    }
    public void TryGrab(PhysicsGrabbable target)
    {
        if (isHolding) return;

        // caluclate forward dist from camroot to target
        float grabbableDistanceFromCamRoot = (target.transform.position - CamRoot.transform.position).magnitude;


        // set that number to the handleDistFromCamera.
        handleDistanceFromCamera = grabbableDistanceFromCamRoot;


        // target.GetGrabbed(this);
        GrabGrabbable(target);
        held = target;

        playerController.SetSpeedMultiplier(target.GrabSettings.SpeedMultiplier);

    }

    public void Drop()
    {
        Destroy(handleConfigurableJoint);
        HandleDrop();
    }
    void DroppedByPhysics()
    {
        HandleDrop();
    }

    void HandleDrop()
    {
        held.Drop();
        held = null;
        playerController.SetSpeedMultiplier(1);
        handlePitch = 0;
        handleYaw = 0;
    }
    public void GrabGrabbable(PhysicsGrabbable grabbable)
    {
        PhysicsGrabSettings grabbableSettings = grabbable.GrabSettings;
        handleConfigurableJoint = GrabberHandle.transform.AddComponent<ConfigurableJoint>();
        handleConfigurableJoint.autoConfigureConnectedAnchor = false;
        handleConfigurableJoint.anchor = Vector3.zero;
        handleConfigurableJoint.connectedAnchor = Vector3.zero;

        // motion
        handleConfigurableJoint.xMotion = ConfigurableJointMotion.Limited;
        handleConfigurableJoint.yMotion = ConfigurableJointMotion.Limited;
        handleConfigurableJoint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = grabbableSettings.Limit;
        handleConfigurableJoint.linearLimit = limit;

        if (grabbableSettings.InfiniteBreakForceAndTorque)
        {
            handleConfigurableJoint.breakForce = Single.PositiveInfinity;
            handleConfigurableJoint.breakTorque = Single.PositiveInfinity;
        }
        else
        {
            handleConfigurableJoint.breakForce = grabbableSettings.BreakForce;
            handleConfigurableJoint.breakTorque = grabbableSettings.BreakTorque;
        }

        JointDrive xDrive = handleConfigurableJoint.xDrive;
        xDrive.positionSpring = grabbableSettings.PositionSpring;
        xDrive.positionDamper = grabbableSettings.PositionDamper;
        handleConfigurableJoint.xDrive = xDrive;

        JointDrive yDrive = handleConfigurableJoint.yDrive;
        yDrive.positionSpring = grabbableSettings.PositionSpring;
        yDrive.positionDamper = grabbableSettings.PositionDamper;
        handleConfigurableJoint.yDrive = yDrive;

        JointDrive zDrive = handleConfigurableJoint.zDrive;
        zDrive.positionSpring = grabbableSettings.PositionSpring;
        zDrive.positionDamper = grabbableSettings.PositionDamper;
        handleConfigurableJoint.zDrive = zDrive;

        // angular motion
        handleConfigurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
        handleConfigurableJoint.angularYMotion = ConfigurableJointMotion.Limited;
        handleConfigurableJoint.angularZMotion = ConfigurableJointMotion.Limited;


        // angular limits
        SoftJointLimit softJointLimit = new();
        softJointLimit.limit = 1;
        handleConfigurableJoint.lowAngularXLimit = softJointLimit;
        handleConfigurableJoint.highAngularXLimit = softJointLimit;
        handleConfigurableJoint.angularYLimit = softJointLimit;
        handleConfigurableJoint.angularZLimit = softJointLimit;



        handleConfigurableJoint.projectionMode = JointProjectionMode.None;


        handleConfigurableJoint.connectedBody = grabbable.Rb;

    }

    public void SecondaryInteract(InteractionContext context)
    {
        if (held == null) return;
        held.SecondaryInteract(context);
    }




    void HandleHandleRotation()
    {
        if (Rotate.action.IsPressed())
        {
            Vector2 mouseDelta = Mouse.action.ReadValue<Vector2>();
            handlePitch += mouseDelta.y * RotateSensitivity;
            handleYaw += mouseDelta.x * RotateSensitivity;

        }

        // rotate through rigidbody in local space.
        Quaternion localRot = Quaternion.Euler(handlePitch, handleYaw, 0);
        Quaternion worldRot = GrabberHandle.transform.parent.rotation * localRot;

        GrabberHandle.HandleRb.MoveRotation(worldRot);



    }





}