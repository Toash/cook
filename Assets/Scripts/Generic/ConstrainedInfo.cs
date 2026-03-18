using UnityEngine;
public enum ConstrainType
{
    Truck

}

/// <summary>
/// Info for constraints, contains information for getting in and out positions.
/// 
/// </summary>
public class ConstrainedInfo
{
    public IInteractable Constrainer;
    public ConstrainType Type;

    // the transform for constraining 
    public Transform Constraint;
    // the transform for unconstraining
    public Transform UnConstraint;

    public ConstrainedInfo(IInteractable Constrainer, ConstrainType Type, Transform Constraint, Transform UnConstraint)
    {
        this.Constrainer = Constrainer;
        this.Type = Type;
        this.Constraint = Constraint;
        this.UnConstraint = UnConstraint;
    }
}