using System.Collections.Generic;

/// <summary>
/// Provides a list of interact infos for an interactable to show what actions are avaliable.
/// </summary>
public interface IInteractInfoProvider
{
    public List<InteractInfo> GetInteractInfos();

}