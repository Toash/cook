using UnityEngine;
public class ConstrainedContext
{
    public IConstrainer Constrainer;

    public Transform Constraint;
    public Transform UnConstraint;

    public ConstrainedContext(IConstrainer Constrainer, Transform Constraint, Transform UnConstraint)
    {
        this.Constrainer = Constrainer;
        this.Constraint = Constraint;
        this.UnConstraint = UnConstraint;
    }
}