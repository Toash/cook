using UnityEngine;

[CreateAssetMenu(fileName = "Grab Settings", menuName = "Grabbable / Grab Settings")]
public class GrabSettings : ScriptableObject
{
    public float PositionSpring = 80;
    public float PositionDamper = 20;

    public float Limit = 5;

    public float BreakForce = 100;
    public float BreakTorque = 100;

    public float SpeedMultiplier = 1;
}