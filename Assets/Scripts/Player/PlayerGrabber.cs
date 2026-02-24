using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerGrabber : MonoBehaviour
{
    public GrabSettings GrabSettings;
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

    private ConfigurableJoint handleJoint;

    void OnEnable()
    {
        GrabberHandle.DroppedByPhysics += OnHandleDrop;
    }
    void OnDisable()
    {
        GrabberHandle.DroppedByPhysics -= OnHandleDrop;
    }


    void Update()
    {
        GrabLight.SetActive(isHolding);
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
            Drop(context);
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
    public void GrabGrabbable(Grabbable grabbable)
    {
        handleJoint = GrabberHandle.transform.AddComponent<ConfigurableJoint>();
        handleJoint.autoConfigureConnectedAnchor = false;
        handleJoint.anchor = Vector3.zero;
        handleJoint.connectedAnchor = Vector3.zero;

        handleJoint.xMotion = ConfigurableJointMotion.Limited;
        handleJoint.yMotion = ConfigurableJointMotion.Limited;
        handleJoint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit limit = new SoftJointLimit();
        limit.limit = GrabSettings.Limit;
        handleJoint.linearLimit = limit;

        handleJoint.breakForce = GrabSettings.BreakForce;
        handleJoint.breakTorque = GrabSettings.BreakTorque;

        handleJoint.angularXMotion = ConfigurableJointMotion.Locked;
        handleJoint.angularYMotion = ConfigurableJointMotion.Locked;
        handleJoint.angularZMotion = ConfigurableJointMotion.Locked;

        JointDrive xDrive = handleJoint.xDrive;
        xDrive.positionSpring = GrabSettings.PositionSpring;
        xDrive.positionDamper = GrabSettings.PositionDamper;
        handleJoint.xDrive = xDrive;

        JointDrive yDrive = handleJoint.yDrive;
        yDrive.positionSpring = GrabSettings.PositionSpring;
        yDrive.positionDamper = GrabSettings.PositionDamper;
        handleJoint.yDrive = yDrive;

        JointDrive zDrive = handleJoint.zDrive;
        zDrive.positionSpring = GrabSettings.PositionSpring;
        zDrive.positionDamper = GrabSettings.PositionDamper;
        handleJoint.zDrive = zDrive;


        handleJoint.connectedBody = grabbable.Rb;

    }

    public void OnHandleDrop()
    {
        held = null;
        playerController.SetSpeedMultiplier(1);
    }
    public void Drop(InteractionContext context)
    {
        // held.Drop();
        Destroy(handleJoint);
        held.Drop(context);
        held = null;

        playerController.SetSpeedMultiplier(1);
        handlePitch = 0;
        handleYaw = 0;
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
        GrabberHandle.HandleRb.MoveRotation(Quaternion.Euler(handlePitch, handleYaw, 0));


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