/// <summary>
/// Class that contains the information for what is possible for an interaction.
/// </summary>
[System.Serializable]
public class InteractInfo
{
    public InteractType Type;
    public string Description;

    public InteractInfo(InteractType type, string description)
    {
        this.Type = type;
        this.Description = description;
    }


}