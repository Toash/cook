using UnityEngine;

/// <summary>
/// Contains references to the player, and its components
/// </summary>
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerGrabber))]
[RequireComponent(typeof(PlayerInteraction))]
public class Player : MonoBehaviour
{
    public PlayerController Controller;
    public PlayerGrabber Grabber;
    public PlayerInteraction Interaction;

    // void Awake()
    // {
    //     Controller = GetComponent<PlayerController>();
    //     Grabber = GetComponent<PlayerGrabber>();
    //     Interaction = GetComponent<PlayerInteraction>();
    // }

}