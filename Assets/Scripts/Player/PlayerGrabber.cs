using System;
using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerGrabber : MonoBehaviour
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

    Grabbable held;
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
    public void OnInteractAndHolding(InteractionContext context)
    {
        if (context.Type == InteractType.Primary)
        {
            DropByInteraction(context);
        }
        else if (context.Type == InteractType.Secondary)
        {
            SecondaryInteract(context);

        }

    }
    public void TryGrab(Grabbable target)
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

    void DropByInteraction(InteractionContext context)
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
    public void GrabGrabbable(Grabbable grabbable)
    {
        GrabSettings grabbableSettings = grabbable.GrabSettings;
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



        // // spring
        // SoftJointLimitSpring softJointLimitSpring = handleConfigurableJoint.angularXLimitSpring;
        // // softJointLimitSpring.damper= 1;
        // handleConfigurableJoint.angularXLimitSpring = softJointLimitSpring;
        // handleConfigurableJoint.angularYZLimitSpring = softJointLimitSpring;



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

            // Handle.transform.localRotation = Quaternion.Euler(handlePitch, handleYaw, 0);
        }

        // rotate through rigidbody in local space.
        Quaternion localRot = Quaternion.Euler(handlePitch, handleYaw, 0);
        Quaternion worldRot = GrabberHandle.transform.parent.rotation * localRot;

        GrabberHandle.HandleRb.MoveRotation(worldRot);

        // GrabberHandle.HandleRb.MoveRotation(Quaternion.Euler(handlePitch, handleYaw, 0));


    }

    // void HandleHandleRotation()
    // {
    //     bool rotating = Rotate.action.IsPressed();

    //     if (rotating && !wasRotating)
    //         targetRot = GrabberHandle.HandleRb.rotation;

    //     if (rotating)
    //     {
    //         Vector2 md = Mouse.action.ReadValue<Vector2>();

    //         float yawDeg = md.x * RotateSensitivity;
    //         float pitchDeg = -md.y * RotateSensitivity;

    //         // yaw around world up
    //         var yaw = Quaternion.AngleAxis(yawDeg, Vector3.up);

    //         // pitch around handle's local right axis (after yaw)
    //         var right = (yaw * GrabberHandle.HandleRb.rotation) * Vector3.right;
    //         var pitch = Quaternion.AngleAxis(pitchDeg, right);

    //         targetRot = pitch * yaw * targetRot;

    //         GrabberHandle.HandleRb.MoveRotation(targetRot);
    //     }

    //     wasRotating = rotating;
    // }




}