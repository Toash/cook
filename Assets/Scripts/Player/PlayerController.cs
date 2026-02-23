using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MouseSens = .35f;
    public float MoveSpeed = 7;

    private float speedMultiplier = 1;
    public float Gravity = 25;
    public float JumpForce = 6;

    public CharacterController CharController;
    public Transform CamRoot;

    public InputActionReference Move;
    public InputActionReference Look;
    public InputActionReference Jump;
    public InputActionReference RightClick;


    Vector3 moveVelocity = Vector3.zero;
    Vector3 gravityVelocity = Vector3.zero;


    float yaw = 0;
    float pitch = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleGravity();
        HandleJumping();
        HandleMovement();
        HandleLooking();


        Vector3 totalVelocity = moveVelocity + gravityVelocity;
        CharController.Move(totalVelocity * Time.deltaTime);

    }

    void HandleGravity()
    {
        if (CharController.isGrounded)
        {
            gravityVelocity.y = -2f;
        }

        gravityVelocity.y -= Gravity * Time.deltaTime;
    }

    void HandleJumping()
    {
        if (Jump.action.WasPressedThisFrame())
        {
            gravityVelocity.y = JumpForce;
        }

    }
    void HandleMovement()
    {
        Vector2 input = Move.action.ReadValue<Vector2>();
        input.Normalize();
        // moveVelocity = new Vector3(input.x, 0, input.y) * moveSpeed;
        moveVelocity = ((transform.right * input.x) + (transform.forward * input.y)) * MoveSpeed * speedMultiplier;

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


}
