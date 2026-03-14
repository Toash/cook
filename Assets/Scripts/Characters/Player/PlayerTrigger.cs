using System;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerTrigger : MonoBehaviour
{
    // public UnityEvent<Player> PlayerEntered;
    // public UnityEvent<Player> PlayerExited;
    public UnityEvent<Player> PlayerEntered;
    public UnityEvent<Player> PlayerExited;
    Collider col;
    Rigidbody rb;
    void OnValidate()
    {
        if (col == null)
        {
            col = GetComponent<Collider>();
            col.isTrigger = true;
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            PlayerEntered?.Invoke(player);

        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            PlayerExited?.Invoke(player);

        }
    }





}