
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public enum JointPriority
{
    FOOD = 0,
    CONTAINER = 1,
}

public class SnapConnection
{
    public FixedJoint Joint;
    public FoodSnapper This;
    public FoodSnapper Other;
    public bool IsOwner;
}

/// <summary>
/// Handles snapping food together to make a FoodRoot.
/// 
/// Can be grabbed, detached ( if attached to another snapper ), or dropped.
/// 
/// Snapping should only occur when the player is actually holding the item. 
/// 
/// Detached can happen both manually from the player, and by physics (from joint breaking)
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Grabbable))]
public class FoodSnapper : MonoBehaviour
{

    [Tooltip("Auto set if not set.")]
    public FoodSnapSettings SnapSettings;
    [Header("Audio")]
    public AudioDefinition SnapSound;
    public AudioDefinition DetachSound;

    [Header("References")]
    private Grabbable grabbable;
    [Tooltip("Determines the snapping priority. Goes from lowest priority to highest priority")]
    public JointPriority JointPriority;

    Rigidbody rb;

    // public Dictionary<JointPriority, FixedJoint> JointsDict = new Dictionary<JointPriority, FixedJoint>();
    // public Dictionary<JointPriority, SnapConnection> SnapConnections = new Dictionary<JointPriority, SnapConnection>();
    public Dictionary<JointPriority, List<SnapConnection>> SnapConnections = new Dictionary<JointPriority, List<SnapConnection>>();


    public event Action<FoodIngredient> OnSnapEvent;
    public event Action<FoodIngredient> OnDetachedEvent;



    private float timeSinceGrabbedDelay = .3f;

    private float initialSnapDelay = 0f; // delay before it actual is snapped.
    private float detachDelay = .5f;

    // timers
    private float timeSinceGrabbed;
    private float snapTimer;
    private float detachTimer;
    private bool canSnap
    {
        // get
        // {
        //     return passedDropDelayTimer || (grabbable.IsBeingHeld() && passedDetachDelayTimer && passedGrabDelayTimer);

        // }
        get
        {
            return passedDetachAndDropDelayTimer || (grabbable.IsBeingHeld() && passedDetachAndDropDelayTimer && passedGrabDelayTimer);

        }
    }


    private bool passedDetachAndDropDelayTimer// ensures that objects dont keep snapping after being detached.
    {
        get
        {
            return detachTimer >= detachDelay;
        }

    }

    private bool passedGrabDelayTimer// ensures the objects dont immediately snap together when grabbing.
    {
        get
        {
            return timeSinceGrabbed >= timeSinceGrabbedDelay;
        }
    }

    // period of time after detaching in which the food can attach. (for dropping food onto another.)
    // private bool

    void OnValidate()
    {
        if (grabbable == null)
        {
            grabbable = GetComponent<Grabbable>();
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        // ingredient = GetComponent<FoodIngredient>();


        // init snap connections for each priority.
        foreach (JointPriority p in Enum.GetValues(typeof(JointPriority)))
        {
            SnapConnections[p] = new List<SnapConnection>();
        }

        if (SnapSound == null)
        {
            SnapSound = Resources.Load<AudioDefinition>("ScriptableObjects/AudioDefinition/Snap");
        }
        if (DetachSound == null)
        {
            DetachSound = Resources.Load<AudioDefinition>("ScriptableObjects/AudioDefinition/Detach");
        }

        if (SnapSettings == null)
        {
            SnapSettings = Resources.Load<FoodSnapSettings>("ScriptableObjects/FoodSnapSettings/Default");

        }

    }

    void Update()
    {
        detachTimer += Time.deltaTime;
        timeSinceGrabbed += Time.deltaTime;
    }

    void FixedUpdate()
    {
        CleanupBrokenConnections();
    }

    void OnEnable()
    {

        grabbable.OnGrab += OnGrabbableGrab;
        grabbable.OnSecondaryInteract += OnGrabbableSecondaryInteract;
        grabbable.OnDrop += OnGrabbableDrop;
    }
    void OnDisable()
    {
        grabbable.OnGrab -= OnGrabbableGrab;
        grabbable.OnSecondaryInteract -= OnGrabbableSecondaryInteract;
        grabbable.OnDrop -= OnGrabbableDrop;

    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision enter");
        if (collision.rigidbody != null)
        {
            if (collision.rigidbody.TryGetComponent<FoodSnapper>(out FoodSnapper snapper))
            {
                snapTimer = 0;
            }
        }

    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            if (collision.rigidbody.TryGetComponent<FoodSnapper>(out FoodSnapper snapper))
            {
                snapTimer += Time.deltaTime;

                if (canSnap)
                {
                    SnapByHighestPriority(snapper);
                }
            }
        }

    }

    void OnJointBreak(float breakForce)
    {
        Debug.Log("Joint has been broken with force " + breakForce);

        // ensure snap connections are updated.
        CleanupBrokenConnections();
    }


    void OnSnap(FoodSnapper other)
    {
        Debug.Log("Snap");
        AudioManager.I.PlayOneShot(SnapSound, transform.position);

        // OnSnapEvent.Invoke(other.ingredient);
        // smelly
        OnSnapEvent.Invoke(other.GetComponent<FoodIngredient>());
    }

    void OnDetached(FoodSnapper other)
    {
        // OnDetachedEvent.Invoke(other.ingredient);
        AudioManager.I.PlayOneShot(DetachSound, transform.position);
        OnDetachedEvent.Invoke(other.GetComponent<FoodIngredient>());

        detachTimer = 0;
        other.detachTimer = 0;

    }


    void OnGrabbableGrab(InteractionContext context)
    {
        snapTimer = 0;
        timeSinceGrabbed = 0;
    }
    void OnGrabbableSecondaryInteract(InteractionContext context)
    {
        DetachJointsByHighestPriority();
    }
    void OnGrabbableDrop(InteractionContext context)
    {

    }




    void SnapByHighestPriority(FoodSnapper other)
    {
        // only make one joint. So this only gets called once between two joints.
        // if (GetInstanceID() > other.GetInstanceID()) return;

        JointPriority highestPriority = (JointPriority)Math.Max((int)JointPriority, (int)other.JointPriority);
        // Check for existing connection on other
        if (SnapConnections.TryGetValue(highestPriority, out var list) && list.Any(c => c.Other == other))
        {
            return;
        }

        // create joint
        Rigidbody otherRb = other.rb;
        FixedJoint joint = gameObject.AddComponent<FixedJoint>();

        joint.breakForce = SnapSettings.BreakForce;
        joint.breakTorque = SnapSettings.BreakTorque;

        joint.connectedBody = otherRb;



        List<SnapConnection> snapConnections = SnapConnections[highestPriority];
        snapConnections.Add(new SnapConnection
        {
            Joint = joint,
            This = this,
            Other = other,
            IsOwner = true
        });
        List<SnapConnection> otherSnapConnections = other.SnapConnections[highestPriority];
        otherSnapConnections.Add(new SnapConnection
        {
            Joint = joint,
            This = other,
            Other = this,
            IsOwner = false
        });

        OnSnap(other);
    }

    public void DetachJointsByHighestPriority()
    {
        // loop through highest priority first.
        foreach (JointPriority jointType in Enum.GetValues(typeof(JointPriority)).Cast<JointPriority>().Reverse())
        {
            if (DetachAllConnectedJointsByType(jointType))
            {
                return;
            }

        }
    }
    /// <summary>
    /// Destroys the joints where the other connections point to this FoodSnapper for a given type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool DetachAllConnectedJointsByType(JointPriority type)
    {
        if (SnapConnections.TryGetValue(type, out List<SnapConnection> connections))
        {
            // get snap connections that reference this snapper.
            List<SnapConnection> allOtherConnections = GetOtherConnectionsThatConnectToThisSnapper(type);

            if (allOtherConnections.Count == 0) return false;


            // disconnect the connections that reference to this snapper from both sides.
            foreach (SnapConnection otherConnection in allOtherConnections)
            {

                List<SnapConnection> otherConnections = otherConnection.This.SnapConnections[type];
                List<SnapConnection> thisConnections = SnapConnections[type];

                otherConnections.RemoveAll(c => c.Other == this);
                thisConnections.RemoveAll(c => c.Other == otherConnection.This);


                // destroy the joint.
                Debug.Log("Destroy joint");
                Destroy(otherConnection.Joint);


                OnDetached(otherConnection.This);


            }



            return true;
        }
        return false;

    }


    /// <summary>
    /// Gets all of the Snap Connections on this snapper, that point back to this snapper.
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public List<SnapConnection> GetOtherConnectionsThatConnectToThisSnapper(JointPriority p)
    {
        var result = new List<SnapConnection>();

        if (!SnapConnections.TryGetValue(p, out var connections))
            return result;

        foreach (var connection in connections)
        {

            if (connection.Other.SnapConnections.TryGetValue(p, out var otherList))
            {
                result.AddRange(otherList.Where(c => c.Other == this));
            }
        }

        return result;
    }






    /// <summary>
    /// 
    /// Loops through snap connections without a valid joint and removes them.
    /// </summary>
    void CleanupBrokenConnections()
    {
        foreach (var key in SnapConnections)
        {
            CleanupBrokenConnectionsByPriority(key.Key);
        }
    }
    void CleanupBrokenConnectionsByPriority(JointPriority priority)
    {
        if (!SnapConnections.TryGetValue(priority, out var list))
            return;

        // loop through the connections that we have and remove any that have a null Joint. do this on both sides.
        // loop backwards beacuse we are removing items whilst iterating this
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var connection = list[i];

            if (connection.Joint == null)
            {
                // Remove from this side
                list.RemoveAt(i);


                // Remove from other side
                if (connection.Other != null &&
                    connection.Other.SnapConnections.TryGetValue(priority, out var otherList))
                {

                    // remove the other connection.
                    var otherConnection = otherList.FirstOrDefault(c => c.Other == this);
                    otherList.Remove(otherConnection);

                    OnDetached(connection.Other);
                }
            }
        }
    }




#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.orange;

        string message = "Can snap: " + canSnap;
        message += "\nSnap Connections: " + SnapConnections.Values.Sum(list => list.Count) + " \n";

        Handles.Label(transform.position, message);

        if (canSnap)
        {

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one * (grabbable.GizmoSize + .1f));
        }

        Gizmos.color = Color.blue;

        // visualize snap connections
        foreach (JointPriority key in SnapConnections.Keys)
        {
            foreach (SnapConnection connection in SnapConnections[key])
            {
                // Gizmos.DrawLine(connection.This.transform.position, connection.Other.transform.position);
                Vector3 a = connection.This.transform.position;
                Vector3 b = connection.Other.transform.position;
                Vector3 dir = a - b;
                Vector3 otherDir = b - a;

                float length = (dir).magnitude;

                Vector3 center = a + (otherDir / 2);

                Gizmos.DrawSphere(center, length);

            }
        }
    }
#endif


}