using UnityEngine;

/// <summary>
/// Contains references to the player, and its components
/// </summary>
public class Player : MonoBehaviour
{
    public PlayerController Controller;
    public PlayerItemHolder ItemHolder;
    // public PlayerPhysicsGrabber PhysicsGrabber;
    public PlayerInteraction Interaction;
    // public PlayerCrosshair Crosshair;

    // void Awake()
    // {
    //     Controller = GetComponent<PlayerController>();
    //     Grabber = GetComponent<PlayerGrabber>();
    //     Interaction = GetComponent<PlayerInteraction>();
    // }

}