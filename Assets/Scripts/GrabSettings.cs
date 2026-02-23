using UnityEngine;

[CreateAssetMenu(fileName = "Grab Settings", menuName = "Grabbable / Grab Settings")]
public class GrabSettings : ScriptableObject
{
    public float PositionSpring = 80;
    public float PositionDamper = 20;

    public float SpeedMultiplier = 1;
}