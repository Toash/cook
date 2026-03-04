using System.Numerics;
using UnityEngine;

public enum InteractType
{
    Primary,
    Secondary

}
public class InteractionContext
{
    public InteractType Type;
    public Player Player;
    public PlayerController Controller;
    public PlayerInteraction Interaction;
    public PlayerPhysicsGrabber Grabber;

    public InteractionContext(InteractType type, Player player)
    {
        this.Type = type;
        this.Player = player;
    }

}