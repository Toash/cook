/// <summary>
///  Interface for things that can constrain.
/// 
/// Can be accessed through a ConstrainedContext.
/// </summary>
public interface IConstrainer
{

    /// <summary>
    /// The interaction that shouild occur when on this constraint.
    /// </summary>
    /// <param name="context"></param>
    public void ConstraintInteract(InteractionContext context);
    public void OnUnConstrained();
}