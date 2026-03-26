using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;


/// <summary>
/// The mode that the player is in. Determines what are currently active in the player controller
/// </summary>
public enum PlayerMode
{
    FullGameplay, // default gameplay mode
    BodyConstrained, // when the player should be physically constrained. For example, whilst driving
    InPopup, // when the player is in a ui popup

}

/// <summary>
/// Contains functionality for physically controlling the player, and managing ui.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float MouseSens = .35f;
    public float MoveSpeed = 7;
    public float SprintSpeedMultiplier = 1.8f;
    public float Acceleration = 2f;

    private float speedMultiplier = 1;
    public float Gravity = 25;
    public float JumpForce = 6;
    public float GroundCheckDistance = 1.5f;

    public float ConstrainSpeed = 10;

    [Header("References")]
    public CharacterController CharController;
    [Tooltip("What gets rotated by player mouse movement. Camera will get set at an offset from this for third person")]
    public Transform CamPivotPoint;
    public Transform CamRoot;
    public LayerMask GroundMask;

    [Header("Actions")]
    public InputActionReference Move;
    public InputActionReference Look;
    public InputActionReference Jump;
    public InputActionReference RightClick;
    public InputActionReference SprintKey;
    public InputActionReference ThirdPerson;

    // events
    public event System.Action<IInteractable> BodyConstrained;
    public event System.Action<IInteractable> BodyUnconstrained;
    public event System.Action CameraConstrained;
    public event System.Action CameraUnconstrained;



    public event System.Action<PopupType> PopupShow;
    public event System.Action<PopupType> PopupHide;

    // the current popup that is being displayed, if the player is in a popup.
    private PopupType currentPopupType;


    public PlayerMode CurrentControlMode { get; private set; }

    Vector3 moveVelocity = Vector3.zero;
    Vector3 wishVelocity = Vector3.zero;
    Vector3 gravityVelocity = Vector3.zero;


    float yaw = 0;
    float pitch = 0;
    bool thirdPerson = false;


    Vector3 initialLocalCameraPos;
    Vector3 constrainedCameraPos;
    Quaternion constrainedCameraRot;

    ConstrainedInfo constraintInfo;
    bool sprinting;
    private bool popupAllowsMovement;

    public IInteractable CurrentConstrainerInteractable { get; private set; }// the constrainer that the player is currently in



    // public bool IsBodyContrained { get; private set; } = false;
    // public bool IsCameraContrained { get; private set; } = false;

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
        // HandleCameraConstraint();
        switch (CurrentControlMode)
        {
            case PlayerMode.FullGameplay:
                HandleGravity();
                HandleJumping();
                HandleWishVelocity();
                HandleAcceleratingVelocity();
                HandleLooking();
                break;
            case PlayerMode.BodyConstrained:
                moveVelocity = Vector3.zero;
                HandleGravity();
                HandleLooking();
                break;
            case PlayerMode.InPopup:
                if (!popupAllowsMovement)
                {
                    moveVelocity = Vector3.zero;
                }

                HandleGravity();

                if (popupAllowsMovement)
                {
                    HandleJumping();
                    HandleWishVelocity();
                    HandleAcceleratingVelocity();
                }

                break;
        }




        if (CharController.enabled)
        {

            Vector3 totalVelocity = moveVelocity + gravityVelocity;
            CharController.Move(totalVelocity * Time.deltaTime);
        }

    }





    void SetPlayerMode(PlayerMode mode)
    {
        Debug.Log("[PlayerController]: Setting player mode to " + mode.ToString());
        var from = CurrentControlMode;
        switch (mode)
        {
            case PlayerMode.FullGameplay:
                Cursor.lockState = CursorLockMode.Locked;
                break;

            case PlayerMode.BodyConstrained:
                break;

            case PlayerMode.InPopup:
                Cursor.lockState = CursorLockMode.None;
                break;

        }
        CurrentControlMode = mode;


    }


    /// <summary>
    /// Only allows one popup at a time
    /// </summary>
    /// <param name="type"></param>
    public void ShowPopup(PopupType type, bool allowMovement = false)
    {
        if (CurrentControlMode == PlayerMode.InPopup) return;
        if (CurrentControlMode != PlayerMode.FullGameplay) return;

        if (type == PopupType.None) return;
        if (currentPopupType != PopupType.None) CloseCurrentPopup();

        popupAllowsMovement = allowMovement;

        PopupShow?.Invoke(type);
        currentPopupType = type;

        SetPlayerMode(PlayerMode.InPopup);
    }

    public void CloseCurrentPopup()
    {
        if (CurrentControlMode != PlayerMode.InPopup) return;

        PopupHide?.Invoke(currentPopupType);
        currentPopupType = PopupType.None;

        SetPlayerMode(PlayerMode.FullGameplay);
    }

    /// <summary>
    /// Lock player pos onto transform. 
    /// </summary>
    /// <param name="constraint"></param>
    public void ConstrainBody(ConstrainedInfo constrainedInfo)
    {
        if (CurrentControlMode == PlayerMode.BodyConstrained) return;
        Debug.Log("[PlayerController]: Constrained Body");
        // IsBodyContrained = true;
        SetPlayerMode(PlayerMode.BodyConstrained);
        moveVelocity = Vector3.zero;

        // character controller does not like teleporting. disable before doing so.
        CharController.enabled = false;

        this.constraintInfo = constrainedInfo;

        transform.position = constrainedInfo.Constraint.position;
        SetYawPitchFromQuaternion(constrainedInfo.Constraint.localRotation);
        transform.SetParent(constrainedInfo.Constraint);

        CurrentConstrainerInteractable = constrainedInfo.Constrainer;

        BodyConstrained?.Invoke(CurrentConstrainerInteractable);

    }
    public void UnconstrainBody()
    {
        if (CurrentControlMode != PlayerMode.BodyConstrained) return;
        if (constraintInfo == null)
        {
            Debug.LogError("[PlayerController]: Constrained context does not exist when trying to unconstrain.");
            return;
        }
        Debug.Log("[PlayerController]: UnConstrained Body");
        SetPlayerMode(PlayerMode.FullGameplay);
        moveVelocity = Vector3.zero;
        // constrainedContext.Constrainer.OnUnConstrained();
        transform.position = constraintInfo.UnConstraint.position;
        transform.rotation = constraintInfo.UnConstraint.rotation;

        CharController.enabled = true;

        this.constraintInfo = null;
        CurrentConstrainerInteractable = null;


        BodyUnconstrained?.Invoke(CurrentConstrainerInteractable);
    }
    public void OnInteractAndConstraint(InteractionContext context)
    {
        Debug.Log(context);
        CurrentConstrainerInteractable.Interact(context);
        // UnconstrainBody();
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
        sprinting = SprintKey.action.IsPressed();

        Vector2 input = Move.action.ReadValue<Vector2>();
        input.Normalize();
        // moveVelocity = new Vector3(input.x, 0, input.y) * moveSpeed;

        float sprintMult = sprinting ? SprintSpeedMultiplier : 1;
        wishVelocity = ((transform.right * input.x) + (transform.forward * input.y)) * MoveSpeed * sprintMult * speedMultiplier;
        // moveVelocity = ((transform.right * input.x) + (transform.forward * input.y)) * MoveSpeed * speedMultiplier;

    }
    void HandleAcceleratingVelocity()
    {
        moveVelocity = Vector3.MoveTowards(moveVelocity, wishVelocity, Acceleration * Time.deltaTime);


    }
    void HandleLooking()
    {
        if (RightClick.action.IsPressed()) return;
        if (ThirdPerson.action.WasPressedThisFrame())
        {
            thirdPerson = !thirdPerson;
            if (!thirdPerson)
            {
                OnFirstPerson();
            }
            else
            {
                OnThirdPerson();
            }


        }
        Vector2 delta = Look.action.ReadValue<Vector2>();
        yaw += delta.x * MouseSens;
        pitch -= delta.y * MouseSens;

        pitch = Mathf.Clamp(pitch, -89, 89);


        transform.localRotation = Quaternion.Euler(0, yaw, 0);
        // CamRoot.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        CamPivotPoint.transform.localRotation = Quaternion.Euler(pitch, 0, 0);

    }
    public void ForceFirstPerson()
    {
        thirdPerson = false;
        OnFirstPerson();
    }
    public void ForceThirdPerson()
    {
        thirdPerson = true;
        OnThirdPerson();
    }
    void OnFirstPerson()
    {
        CamRoot.localPosition = Vector3.zero;
    }
    void OnThirdPerson()
    {
        if (constraintInfo != null)
        {
            if (constraintInfo.Type == ConstrainType.Truck)
            {
                CamRoot.localPosition = Vector3.back * 9;
                return;
            }

        }
        CamRoot.localPosition = Vector3.back * 4;

    }
    public void SetSpeedMultiplier(float mult)
    {
        speedMultiplier = mult;

    }

    void SetYawPitchFromQuaternion(Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;

        yaw = euler.y;
        pitch = euler.x;
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
