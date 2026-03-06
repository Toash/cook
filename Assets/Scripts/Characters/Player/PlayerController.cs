using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float MouseSens = .35f;
    public float MoveSpeed = 7;
    public float Acceleration = 2f;

    private float speedMultiplier = 1;
    public float Gravity = 25;
    public float JumpForce = 6;
    public float GroundCheckDistance = 1.5f;

    public float ConstrainSpeed = 10;

    [Header("References")]
    public CharacterController CharController;
    public Transform CamRoot;
    public LayerMask GroundMask;

    [Header("Actions")]
    public InputActionReference Move;
    public InputActionReference Look;
    public InputActionReference Jump;
    public InputActionReference RightClick;

    // events
    public event System.Action BodyConstrained;
    public event System.Action BodyUnconstrained;
    public event System.Action CameraConstrained;
    public event System.Action CameraUnconstrained;


    Vector3 moveVelocity = Vector3.zero;
    Vector3 wishVelocity = Vector3.zero;
    Vector3 gravityVelocity = Vector3.zero;


    float yaw = 0;
    float pitch = 0;


    Vector3 initialLocalCameraPos;
    Vector3 constrainedCameraPos;
    Quaternion constrainedCameraRot;

    ConstrainedContext constrainedContext;


    public bool IsBodyContrained { get; private set; } = false;
    public bool IsCameraContrained { get; private set; } = false;

    void Awake()
    {
        initialLocalCameraPos = CamRoot.localPosition;
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleCameraConstraint();

        HandleGravity();
        if (!IsCameraContrained && !IsBodyContrained)
        {
            HandleJumping();
            HandleWishVelocity();
            HandleAcceleratingVelocity();
        }
        if (!IsCameraContrained)
        {
            HandleLooking();
        }



        if (CharController.enabled)
        {
            Vector3 totalVelocity = moveVelocity + gravityVelocity;
            CharController.Move(totalVelocity * Time.deltaTime);
        }

    }

    void HandleCameraConstraint()
    {

        if (IsCameraContrained)
        {
            CamRoot.position = Vector3.Lerp(CamRoot.position, constrainedCameraPos, Time.deltaTime * ConstrainSpeed);
            CamRoot.rotation = Quaternion.Slerp(CamRoot.rotation, constrainedCameraRot, Time.deltaTime * ConstrainSpeed);
        }
        else
        {
            CamRoot.localPosition = Vector3.Lerp(CamRoot.localPosition, initialLocalCameraPos, Time.deltaTime * ConstrainSpeed);
            // CamRoot.rotation = Quaternion.Slerp(CamRoot.rotation, constrainedCameraRot, Time.deltaTime * ConstrainSpeed);

        }


    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Lock player pos onto transform. 
    /// </summary>
    /// <param name="constraint"></param>
    public void ConstrainBody(ConstrainedContext constrainedContext)
    {
        if (IsBodyContrained) return;
        Debug.Log("[PlayerController]: Constrained Body");
        IsBodyContrained = true;
        moveVelocity = Vector3.zero;

        CharController.enabled = false;

        this.constrainedContext = constrainedContext;


        transform.position = constrainedContext.Constraint.position;
        transform.rotation = constrainedContext.Constraint.rotation;
        transform.SetParent(constrainedContext.Constraint);


        BodyConstrained?.Invoke();
    }
    public void UnconstrainBody()
    {
        if (!IsBodyContrained) return;
        Debug.Log("[PlayerController]: UnConstrained Body");
        IsBodyContrained = false;
        moveVelocity = Vector3.zero;

        constrainedContext.Constrainer.OnUnConstrained();
        transform.position = constrainedContext.UnConstraint.position;
        transform.rotation = constrainedContext.UnConstraint.rotation;

        CharController.enabled = true;

        this.constrainedContext = null;


        BodyUnconstrained?.Invoke();
    }
    /// <summary>
    /// Lock camera at a position and rotation. Used for interacting with world screens.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    public void ConstrainCamera(Vector3 pos, Quaternion rot)
    {
        IsCameraContrained = true;
        moveVelocity = Vector3.zero;
        // CamRoot.position = pos;
        // CamRoot.rotation = rot;
        constrainedCameraPos = pos;
        constrainedCameraRot = rot;

        CameraConstrained?.Invoke();
    }

    public void UnConstrainCamera()
    {
        Debug.Log("UnConstrained");
        IsCameraContrained = false;


        // sync the yaw and pitch back.
        yaw = transform.localEulerAngles.y;
        float rawPitch = CamRoot.localEulerAngles.x;
        if (rawPitch > 180f)
            rawPitch -= 360f;

        pitch = rawPitch;

        // REMOVE ME ASAP!@@!@@!@!!@@!
        LockCursor();
        CameraUnconstrained?.Invoke();
    }

    void HandleGravity()
    {
        if (CharController.isGrounded)
        {
            gravityVelocity.y = -2f;
        }

        gravityVelocity.y -= Gravity * Time.deltaTime;
    }


    bool CanJump()
    {
        return Physics.Raycast(transform.position, Vector3.down, GroundCheckDistance, GroundMask);
    }

    /// <summary>
    /// make sure this is called after Handling gravity.
    /// </summary>
    void HandleJumping()
    {
        if (!CanJump()) return;
        if (Jump.action.WasPressedThisFrame())
        {
            Debug.Log("Jump");
            gravityVelocity.y = JumpForce;
        }

    }
    void HandleWishVelocity()
    {
        Vector2 input = Move.action.ReadValue<Vector2>();
        input.Normalize();
        // moveVelocity = new Vector3(input.x, 0, input.y) * moveSpeed;
        wishVelocity = ((transform.right * input.x) + (transform.forward * input.y)) * MoveSpeed * speedMultiplier;
        // moveVelocity = ((transform.right * input.x) + (transform.forward * input.y)) * MoveSpeed * speedMultiplier;

    }
    void HandleAcceleratingVelocity()
    {
        moveVelocity = Vector3.MoveTowards(moveVelocity, wishVelocity, Acceleration * Time.deltaTime);


    }
    void HandleLooking()
    {
        if (RightClick.action.IsPressed()) return;
        Vector2 delta = Look.action.ReadValue<Vector2>();
        yaw += delta.x * MouseSens;
        pitch -= delta.y * MouseSens;

        pitch = Mathf.Clamp(pitch, -89, 89);

        transform.localRotation = Quaternion.Euler(0, yaw, 0);
        CamRoot.transform.localRotation = Quaternion.Euler(pitch, 0, 0);

    }
    public void SetSpeedMultiplier(float mult)
    {
        speedMultiplier = mult;

    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (CanJump())
        {

            Gizmos.color = Color.green;
        }
        else
        {

            Gizmos.color = Color.red;
        }

        Gizmos.DrawRay(transform.position, Vector3.down * GroundCheckDistance);
        Gizmos.DrawWireSphere(transform.position + Vector3.down * GroundCheckDistance, .2f);
        Handles.Label(transform.position + Vector3.down * GroundCheckDistance, "GroundCheck");

    }
#endif


}
