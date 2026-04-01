using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class NearbyPlayerTrigger : MonoBehaviour
{
    [ShowInInspector, ReadOnly]
    public Player NearbyPlayer { get; private set; }


    void OnTriggerEnter(Collider other)
    {
        Debug.Log("[NearbyPlayerTrigger]: trigger entered");
        if (other.TryGetComponent<Player>(out var player))
        {
            NearbyPlayer = player;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            if (NearbyPlayer == player)
                NearbyPlayer = null;
        }
    }
}