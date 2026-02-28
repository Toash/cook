
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;


/// <summary>
/// When detaching, detaches the higher priority 
/// </summary>
public enum JointPriority
{
    FOOD = 0,
    CONTAINER = 1,
}

/// <summary>
/// Connection between two Snappers with a Joint.
/// </summary>
public class SnapConnection
{
    public FixedJoint Joint;
    public Snapper This;
    public Snapper Other;
    public bool IsOwner;

    public override string ToString()
    {
        return "SnapConnection: \n" +
        "Joint: " + Joint + "\n" +
        "This: " + This.gameObject.name + "\n" +
        "Other: " + Other.gameObject.name + "\n" +
        "IsOwner: " + IsOwner;
    }
}

/// <summary>
/// Snaps together with other snappers.<br/>
/// Snappers require a Grabbable. <br/>
/// 
/// Can be grabbed, detached ( if attached to another snapper ), or dropped.<br/>
/// 
/// Snapping should only occur when the player is actually holding the item. <br/>
/// 
/// Detached can happen both manually from the player, and by physics (from joint breaking)
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Grabbable))]
public class Snapper : MonoBehaviour
{

    [Tooltip("Determines the snapping priority. When detaching, detaches the highest priority first.")]
    public JointPriority JointPriority;
    [Tooltip("Auto set if not set.")]
    public SnapperSettings SnapSettings;
    [Header("Audio")]
    public AudioDefinition SnapSound;
    public AudioDefinition DetachSound;

    [Header("References")]
    private Grabbable grabbable;

    Rigidbody rb;

    // public Dictionary<JointPriority, FixedJoint> JointsDict = new Dictionary<JointPriority, FixedJoint>();
    // public Dictionary<JointPriority, SnapConnection> SnapConnections = new Dictionary<JointPriority, SnapConnection>();
    public Dictionary<JointPriority, List<SnapConnection>> SnapConnections = new Dictionary<JointPriority, List<SnapConnection>>();


    // triggered when snapped, passes in other snapper. 
    public event Action<SnapConnection> OnSnapEvent;
    // triggered when detached, passes in other snapper. 
    public event Action<SnapConnection> OnDetachedEvent;



    private float timeSinceGrabbedDelay = .3f;

    private float initialSnapDelay = 0f; // delay before it actual is snapped.
    private float detachDelay = .5f;

    // timers
    private float timeSinceGrabbed;
    private float snapTimer;
    private float detachTimer;
    private bool canSnap
    {
        get
        {
            if (grabbable == null) return false;
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
            SnapSettings = Resources.Load<SnapperSettings>("ScriptableObjects/FoodSnapSettings/Default");

        }
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
    }
    void OnDisable()
    {
        grabbable.OnGrab -= OnGrabbableGrab;
        grabbable.OnSecondaryInteract -= OnGrabbableSecondaryInteract;

    }

    void OnDestroy()
    {
        // cleanup Snap collections 
        foreach (JointPriority priority in SnapConnections.Keys)
        {
            DetachAllConnectedJointsByPriority(priority);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision enter");
        if (collision.rigidbody != null)
        {
            if (collision.rigidbody.TryGetComponent<Snapper>(out Snapper snapper))
            {
                snapTimer = 0;
            }
        }

    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.rigidbody != null)
        {
            if (collision.rigidbody.TryGetComponent<Snapper>(out Snapper snapper))
            {
                snapTimer += Time.deltaTime;

                if (canSnap)
                {
                    SnapByHighestPriority(snapper);
                }
            }
        }

    }

    /// <summary>
    /// Returns all of the snappers connected by snap collections recursively
    /// </summary>
    /// <returns></returns>
    public List<Snapper> GetSnapperGroup(bool includeSelf = true)
    {
        var visited = new HashSet<Snapper>();
        var stack = new Stack<Snapper>();

        visited.Add(this);
        stack.Push(this);

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            foreach (var kv in current.SnapConnections)
            {
                List<SnapConnection> connections = kv.Value;
                if (connections == null) continue;

                for (int i = 0; i < connections.Count; i++)
                {
                    var c = connections[i];
                    if (c == null) continue;

                    Snapper otherSnapper = c.Other;
                    if (otherSnapper == null) continue;

                    if (visited.Add(otherSnapper))
                        stack.Push(otherSnapper);
                }
            }
        }

        if (!includeSelf) visited.Remove(this);
        return visited.ToList();
    }




    void OnJointBreak(float breakForce)
    {
        Debug.Log("[Snapper]: Joint broken with force " + breakForce);

        // ensure snap connections are updated.
        CleanupBrokenConnections();
    }


    /// <summary>
    /// Called when a snapper snaps to another snapper. Called on both snappers
    /// </summary>
    /// <param name="other"></param>
    /// <param name="otherSnapConnection"></param>
    void OnSnap(SnapConnection otherSnapConnection)
    {
        Debug.Log("[Snapper]: Calling OnSnap on GameObject " + gameObject.name);
        if (otherSnapConnection.IsOwner == true)
        {
            AudioManager.I.PlayOneShot(SnapSound, transform.position);
        }

        // OnSnapEvent?.Invoke(other);
        OnSnapEvent?.Invoke(otherSnapConnection);
    }

    /// <summary>
    /// Called when a snapper detaches from another snapper. Called on both snappers
    /// </summary>
    /// <param name="other"></param>
    /// <param name="otherSnapConnection"></param>
    void OnDetached(SnapConnection otherSnapConnection)
    {
        Debug.Log("[Snapper]: Calling OnDetached on GameObject " + gameObject.name);

        if (otherSnapConnection.IsOwner == true)
        {
            AudioManager.I.PlayOneShot(DetachSound, transform.position);
        }

        // OnDetachedEvent?.Invoke(other);
        OnDetachedEvent?.Invoke(otherSnapConnection);
        detachTimer = 0;
        otherSnapConnection.This.detachTimer = 0;

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




    void SnapByHighestPriority(Snapper other)
    {
        if (other == this)
        {
            Debug.LogError("[Snapper]: Cannot snap to same object.");
        }
        // only make one joint. So this only gets called once between two joints.
        // if (GetInstanceID() > other.GetInstanceID()) return;

        JointPriority highestPriority = (JointPriority)Math.Max((int)JointPriority, (int)other.JointPriority);
        // ensure an existing connection doesnt already exist.
        if (SnapConnections.TryGetValue(highestPriority, out var list) && list.Any(c => c.Other == other))
        {
            return;
        }

        // create joint
        Rigidbody otherRb = other.rb;
        FixedJoint fixedJoint = gameObject.AddComponent<FixedJoint>();

        if (SnapSettings.Infinity)
        {
            fixedJoint.breakForce = Single.PositiveInfinity;
            fixedJoint.breakTorque = Single.PositiveInfinity;
        }
        else
        {
            fixedJoint.breakForce = SnapSettings.BreakForce;
            fixedJoint.breakTorque = SnapSettings.BreakTorque;
        }

        fixedJoint.connectedBody = otherRb;
        fixedJoint.enableCollision = false;
        fixedJoint.enablePreprocessing = true;



        // add snap connection to this snapper
        List<SnapConnection> snapConnections = SnapConnections[highestPriority];
        SnapConnection snapConnection =
        new SnapConnection
        {
            Joint = fixedJoint,
            This = this,
            Other = other,
            IsOwner = true
        };
        snapConnections.Add(snapConnection);

        // add snap connection to other snapper
        List<SnapConnection> otherSnapConnections = other.SnapConnections[highestPriority];
        SnapConnection otherSnapConnection = new SnapConnection
        {
            Joint = fixedJoint,
            This = other,
            Other = this,
            IsOwner = false
        };
        otherSnapConnections.Add(otherSnapConnection);

        // OnSnap(this, snapConnection);
        // other.OnSnap(other, otherSnapConnection);

        OnSnap(otherSnapConnection);
        other.OnSnap(snapConnection);
        // OnSnap(other, otherSnapConnection);
    }

    public void DetachJointsByHighestPriority()
    {
        // loop through highest priority first.
        foreach (JointPriority jointType in Enum.GetValues(typeof(JointPriority)).Cast<JointPriority>().Reverse())
        {
            if (DetachAllConnectedJointsByPriority(jointType))
            {
                return;
            }

        }
    }
    /// <summary>
    /// Destroys the joints where the other connections point to this FoodSnapper for a given priority.
    /// </summary>
    /// <param name="priority"></param>
    /// <returns></returns>
    bool DetachAllConnectedJointsByPriority(JointPriority priority)
    {
        if (SnapConnections.TryGetValue(priority, out List<SnapConnection> connections))
        {
            // get snap connections that reference this snapper.
            List<SnapConnection> otherConnectionsToThis = GetOtherConnectionsThatConnectToThisSnapper(priority);

            if (otherConnectionsToThis.Count == 0) return false;


            // disconnect the connections that reference to this snapper from both sides.
            foreach (SnapConnection otherConnection in otherConnectionsToThis)
            {

                // List<SnapConnection> otherConnections = otherConnectionThatPointsToThis.This.SnapConnections[priority];
                // List<SnapConnection> thisConnections = SnapConnections[priority];

                // Snapper otherSnapper = otherConnectionThatPointsToThis.This;

                // // remove other connections that reference this
                // otherConnections.RemoveAll(c => c.Other == this);
                // thisConnections.RemoveAll(c => c.Other == otherSnapper);

                List<SnapConnection> otherConnections = otherConnection.This.SnapConnections[priority];
                List<SnapConnection> thisConnections = SnapConnections[priority];

                Snapper otherSnapper = otherConnection.This;

                // remove the connection on the other snapper
                otherConnections.Remove(otherConnection);



                // remove the connection on this that points to the other snapper
                SnapConnection connectionThatPointsToOther = thisConnections.Find(c => c.Other == otherSnapper);
                thisConnections.Remove(connectionThatPointsToOther);

                // destroy the joint.
                Debug.Log("Destroy joint");
                Destroy(otherConnection.Joint);


                OnDetached(otherConnection);
                otherSnapper.OnDetached(connectionThatPointsToOther);
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
        if (!SnapConnections.TryGetValue(priority, out var snapConnections))
            return;

        // loop through the connections that we have and remove any that have a null Joint. do this on both sides.
        // loop backwards beacuse we are removing items whilst iterating this
        for (int i = snapConnections.Count - 1; i >= 0; i--)
        {
            SnapConnection connection = snapConnections[i];

            if (connection.Joint == null)
            {
                // Remove from this side
                snapConnections.RemoveAt(i);


                // Remove from other side
                if (connection.Other != null &&
                    connection.Other.SnapConnections.TryGetValue(priority, out var otherList))
                {

                    // remove the other connection.
                    SnapConnection otherConnection = otherList.Find(c => c.Other == this);
                    otherList.Remove(otherConnection);

                    OnDetached(otherConnection);
                    connection.Other.OnDetached(connection);
                }
            }
        }
    }




#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {

        // string message = "";
        // message += "Can snap: " + canSnap;
        // message += "\nSnap Connections: " + SnapConnections.Values.Sum(list => list.Count) + " \n";

        // Handles.Label(transform.position, message);

        if (canSnap)
        {

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one * (grabbable.GizmoSize + .1f));
        }

        Gizmos.color = Color.blue;

        // visualize snap connections
        string message = "";
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

                Gizmos.DrawSphere(center, length / 2);


                message += connection.ToString() + "\n";
            }
        }

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.blue;
        style.fontStyle = FontStyle.Bold;
        Handles.Label(transform.position + (Vector3.up * 1), message, style);
    }
#endif


}