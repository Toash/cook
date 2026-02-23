using System;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabberHandle : MonoBehaviour
{
    public Rigidbody HandleRb { get; private set; }

    public event Action DroppedByPhysics;
    void Start()
    {
        HandleRb = GetComponent<Rigidbody>();
        HandleRb.isKinematic = true;
        HandleRb.useGravity = false;

    }
    void OnJointBreak(float breakForce)
    {
        DroppedByPhysics.Invoke();
        Debug.Log("Grabber handle joint broke");
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        {

            Gizmos.DrawWireSphere(transform.position, .2f);
        }

    }
#endif
}