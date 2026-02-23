using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrabber : MonoBehaviour
{
    public PlayerController playerController;
    public float MinCamDist = 1;
    public float MaxCamDist = 3;
    public float ScrollSensitivity = .1f;
    public float RotateSensitivity = .05f;


    public InputActionReference Rotate;
    public InputActionReference Mouse;
    public InputActionReference Scroll;

    public Transform Handle;

    private float handlePitch = 0;
    private float handleYaw = 0;
    public Rigidbody HandleRb { get; private set; }
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

    public void OnInteractAndHolding(InteractionContext context)
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
    public void TryGrab(Grabbable target)
    {
        if (isHolding) return;
        target.GetGrabbed(this);
        held = target;

        playerController.SetSpeedMultiplier(target.GrabSettings.SpeedMultiplier);

    }

    public void Drop()
    {
        held.Drop();
        held = null;

        playerController.SetSpeedMultiplier(1);
    }

    public void SecondaryInteract(InteractionContext context)
    {
        if (held == null) return;
        held.SecondaryInteract(context);
    }

    void Start()
    {
        HandleRb = Handle.GetComponent<Rigidbody>();

        if (HandleRb == null)
        {
            Debug.LogWarning("Handle does not have rigidbody. Generating");
            HandleRb = Handle.AddComponent<Rigidbody>();
        }
        HandleRb.isKinematic = true;
        HandleRb.useGravity = false;

    }

    void Update()
    {
        HandleHandleRotation();
        float scrollDelta = Scroll.action.ReadValue<float>();
        float delta = scrollDelta * ScrollSensitivity;
        handleDistanceFromCamera = Mathf.Clamp(handleDistanceFromCamera += delta, MinCamDist, MaxCamDist);


        Handle.position = CamRoot.transform.position + (CamRoot.forward * handleDistanceFromCamera);
    }

    void HandleHandleRotation()
    {
        if (Rotate.action.IsPressed())
        {
            Vector2 mouseDelta = Mouse.action.ReadValue<Vector2>();
            handlePitch -= mouseDelta.y * RotateSensitivity;
            handleYaw += mouseDelta.x * RotateSensitivity;

            // Handle.transform.localRotation = Quaternion.Euler(handlePitch, handleYaw, 0);
            HandleRb.MoveRotation(Quaternion.Euler(handlePitch, handleYaw, 0));
        }


    }



#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (Handle != null)
        {

            Gizmos.DrawSphere(Handle.position, .2f);
            Handles.Label(Handle.position, "Handle\nDistance from camera: " + handleDistanceFromCamera);
        }

    }
#endif

}