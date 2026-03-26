using System.Collections.Generic;

public interface IInteractable
{
    public void Interact(InteractionContext context);
    public List<InteractInfo> GetHoverInteractInfos();
}