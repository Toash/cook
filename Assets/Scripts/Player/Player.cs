using UnityEngine;

/// <summary>
/// Contains references to the player, and its components
/// </summary>
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerGrabber))]
[RequireComponent(typeof(PlayerInteraction))]
public class Player : MonoBehaviour
{
    public PlayerController Controller { get; private set; }
    public PlayerGrabber Grabber { get; private set; }
    public PlayerInteraction Interaction { get; private set; }

    void Awake()
    {
        Controller = GetComponent<PlayerController>();
        Grabber = GetComponent<PlayerGrabber>();
        Interaction = GetComponent<PlayerInteraction>();
    }

}