using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarControl : MonoBehaviour
{
    public CarSeat Seat;
    public WheelControl[] Wheels;
    [Header("Car Properties")]
    public float motorTorque = 1000f;
    public float brakeTorque = 3000f;
    public float targetTopSpeed = 8f;
    public float steeringRange = 30f;
    public float steeringRangeAtMaxSpeed = 10f;
    public float centreOfGravityOffset = -1f;

    private Rigidbody rigidBody;

    private CarInputActions carControls; // Reference to the new input system

    void OnValidate()
    {
        if (rigidBody == null)
        {
            rigidBody = GetComponent<Rigidbody>();

        }
    }
    void Awake()
    {
        carControls = new CarInputActions(); // Initialize Input Actions
        if (Seat == null)
        {
            Debug.LogError("CarControl: No CarSeat assigned!", this);
        }
    }
    void OnEnable()
    {
        carControls.Enable();
    }

    void OnDisable()
    {
        carControls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();

        // Adjust center of mass to improve stability and prevent rolling
        Vector3 centerOfMass = rigidBody.centerOfMass;
        centerOfMass.y += centreOfGravityOffset;
        rigidBody.centerOfMass = centerOfMass;

        // Get all wheel components attached to the car
        // Wheels = GetComponentsInChildren<WheelControl>();
    }

    void FixedUpdate()
    {
        if (Seat.PlayerInSeat == null)
        {
            return;
        }
        Vector2 inputVector = carControls.Car.Movement.ReadValue<Vector2>();

        float vInput = inputVector.y;
        float hInput = inputVector.x;

        float forwardSpeed = Vector3.Dot(transform.forward, rigidBody.linearVelocity);
        float speedFactor = Mathf.InverseLerp(0, targetTopSpeed, Mathf.Abs(forwardSpeed)); // Normalized speed factor

        // Reduce motor torque and steering at high speeds for better handling
        float currentMotorTorque = Mathf.Lerp(motorTorque, 0, speedFactor);
        float currentSteerRange = Mathf.Lerp(steeringRange, steeringRangeAtMaxSpeed, speedFactor);

        // Determine if the player is accelerating or trying to reverse
        bool isAccelerating = Mathf.Sign(vInput) == Mathf.Sign(forwardSpeed);

        foreach (WheelControl wheel in Wheels)
        {
            // Apply steering to wheels that support steering
            if (wheel.steerable)
            {
                wheel.WheelCollider.steerAngle = hInput * currentSteerRange;
            }

            if (isAccelerating)
            {
                // Apply torque to motorized wheels
                if (wheel.motorized)
                {
                    wheel.WheelCollider.motorTorque = vInput * currentMotorTorque;
                }
                // Release brakes when accelerating
                wheel.WheelCollider.brakeTorque = 0f;
            }
            else
            {
                // Apply brakes when reversing direction
                wheel.WheelCollider.motorTorque = 0f;
                wheel.WheelCollider.brakeTorque = Mathf.Abs(vInput) * brakeTorque;
            }
        }
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // visualize center of mass
        Gizmos.color = Color.green;
        Vector3 centerOfMass = transform.position + (rigidBody.centerOfMass + (Vector3.up * centreOfGravityOffset));

        Gizmos.DrawSphere(centerOfMass, 0.2f);
    }
#endif
}