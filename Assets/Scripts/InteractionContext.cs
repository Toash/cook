public enum InteractType
{
    Primary,
    Secondary

}
public class InteractionContext
{
    public InteractType Type;
    public PlayerInteraction Interaction;
    public PlayerGrabber Grabber;

    public InteractionContext(InteractType type, PlayerInteraction interaction, PlayerGrabber grabber)
    {
        this.Type = type;
        this.Interaction = interaction;
        this.Grabber = grabber;
    }

}