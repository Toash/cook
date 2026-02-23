
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;


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

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FoodIngredient))] // ?
public class FoodSnapper : MonoBehaviour
{

    [Tooltip("Auto set if not set.")]
    public FoodSnapSettings SnapSettings;
    [Header("Audio")]
    public AudioDefinition SnapSound;

    [Header("References")]
    public Grabbable Grabbable;
    [Tooltip("Determines the snapping priority. Goes from lowest priority to highest priority")]
    public JointPriority JointPriority;

    Rigidbody rb;

    // public Dictionary<JointPriority, FixedJoint> JointsDict = new Dictionary<JointPriority, FixedJoint>();
    // public Dictionary<JointPriority, SnapConnection> SnapConnections = new Dictionary<JointPriority, SnapConnection>();
    public Dictionary<JointPriority, List<SnapConnection>> SnapConnections = new Dictionary<JointPriority, List<SnapConnection>>();

    private FoodIngredient ingredient;

    private float snapDelay = .3f;
    private float snapTimer;
    private bool canSnap
    {
        get
        {
            return snapTimer >= snapDelay;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        ingredient = GetComponent<FoodIngredient>();

        // init snap connections for each priority.
        foreach (JointPriority p in Enum.GetValues(typeof(JointPriority)))
        {
            SnapConnections[p] = new List<SnapConnection>();
        }

        if (SnapSound == null)
        {
            SnapSound = Resources.Load<AudioDefinition>("ScriptableObjects/AudioDefinition/Snap");
        }

        if (SnapSettings == null)
        {
            SnapSettings = Resources.Load<FoodSnapSettings>("ScriptableObjects/FoodSnapSettings/Default");

        }

    }

    void FixedUpdate()
    {
        CleanupBrokenConnections();
    }

    void OnEnable()
    {

        Grabbable.OnPrimaryInteract += OnPrimaryInteract;
        Grabbable.OnSecondaryInteract += OnSecondaryInteract;
    }
    void OnDisable()
    {
        Grabbable.OnPrimaryInteract -= OnPrimaryInteract;
        Grabbable.OnSecondaryInteract -= OnSecondaryInteract;

    }
    void OnCollisionEnter(Collision collision)
    {
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


    void OnSnap()
    {
        AudioManager.I.PlayOneShot(SnapSound, transform.position);

    }

    void OnPrimaryInteract(InteractionContext context)
    {
        snapTimer = 0;
    }
    void OnSecondaryInteract(InteractionContext context)
    {
        DetachJointsByHighestPriority();
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

                // TODO is remove all not needed?
                // we could just remove both connections for the otherConnection.
                otherConnections.RemoveAll(c => c.Other == this);
                thisConnections.RemoveAll(c => c.Other == otherConnection.This);


                // destroy the joint.
                Debug.Log("Destroy joint");
                Destroy(otherConnection.Joint);

                // reset timer for the other snappers. 
                otherConnection.This.snapTimer = 0;

                // remove food root on other connections if it exists
                if (otherConnection.This.ingredient.FoodRoot != null)
                {
                    otherConnection.This.ingredient.FoodRoot.RemoveIngredient(otherConnection.Other.ingredient);
                }

            }


            // remove food root if it exists
            if (ingredient.FoodRoot != null)
            {
                ingredient.FoodRoot.RemoveIngredient(ingredient);
            }

            snapTimer = 0;

            return true;
        }
        return false;

    }


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


    void SnapByHighestPriority(FoodSnapper other)
    {
        // only make one joint. So this only gets called once between two joints.
        if (GetInstanceID() > other.GetInstanceID()) return;

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

        OnSnap();


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

        // create food roots
        if (ingredient.FoodRoot != null)
        {
            // add to existing food root
            ingredient.FoodRoot.AddIngredient(other.ingredient);
        }
        else if (other.ingredient.FoodRoot != null)
        {
            // add to existing food root
            other.ingredient.FoodRoot.AddIngredient(ingredient);
        }
        else
        {
            // create food root
            FoodRoot foodRoot = FoodRoot.CreateRootFromIngredient(ingredient);
            foodRoot.AddIngredient(ingredient);
            foodRoot.AddIngredient(other.ingredient);

        }
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
                    otherList.RemoveAll(c => c.Joint == null || c.Other == this);
                }
            }
        }
    }




#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        string message = "Snap timer: " + snapTimer + "\n Can snap: " + canSnap;
        // message += "\nCurrent snap types: \n";
        // foreach (JointPriority priority in Enum.GetValues(typeof(JointPriority)))
        // {
        //     if (SnapConnections.ContainsKey(priority))
        //     {
        //         message += priority.ToString() + "\n";
        //     }
        // }
        message += "\nSnap Connections: " + SnapConnections.Values.Sum(list => list.Count) + " \n";

        Handles.Label(transform.position, message);
    }
#endif


}