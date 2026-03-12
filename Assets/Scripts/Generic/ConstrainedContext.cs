using UnityEngine;
public enum ConstrainType
{
    Truck

}

public class ConstrainedContext
{
    public IConstrainer Constrainer;
    public ConstrainType Type;

    // the transform for constraining 
    public Transform Constraint;
    // the transform for unconstraining
    public Transform UnConstraint;

    public ConstrainedContext(IConstrainer Constrainer, ConstrainType Type, Transform Constraint, Transform UnConstraint)
    {
        this.Constrainer = Constrainer;
        this.Type = Type;
        this.Constraint = Constraint;
        this.UnConstraint = UnConstraint;
    }
}