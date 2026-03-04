using UnityEngine;

[CreateAssetMenu(fileName = "Grab Settings", menuName = "Grabbable / Grab Settings")]
public class PhysicsGrabSettings : ScriptableObject
{
    public float PositionSpring = 80;
    public float PositionDamper = 20;

    public float Limit = 5;

    public bool InfiniteBreakForceAndTorque = false;

    public float BreakForce = 100;
    public float BreakTorque = 100;

    public float SpeedMultiplier = 1;
}