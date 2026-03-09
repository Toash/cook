using UnityEngine;
public class ConstrainedContext
{
    public IConstrainer Constrainer;

    // the transform for constraining 
    public Transform Constraint;
    // the transform for unconstraining
    public Transform UnConstraint;

    public ConstrainedContext(IConstrainer Constrainer, Transform Constraint, Transform UnConstraint)
    {
        this.Constrainer = Constrainer;
        this.Constraint = Constraint;
        this.UnConstraint = UnConstraint;
    }
}