public enum InteractType
{
    Primary,
    Secondary

}
public class InteractionContext
{
    public InteractType Type;
    public PlayerController Controller;
    public PlayerInteraction Interaction;
    public PlayerGrabber Grabber;

    public InteractionContext(InteractType type, PlayerController controller, PlayerInteraction interaction, PlayerGrabber grabber)
    {
        this.Type = type;
        this.Controller = controller;
        this.Interaction = interaction;
        this.Grabber = grabber;
    }

}