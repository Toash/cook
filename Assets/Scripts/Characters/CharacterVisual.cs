using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Handles the visual representation of a character
/// 
/// </summary>
public class CharacterVisual : MonoBehaviour
{

    [Header("Properties")]
    public float HorizontalRootLookAtSpeed = 10f;

    [Header("References")]
    public Transform VisualRoot;
    public Moveable Moveable;
    public Animator Animator;
    public string MovementBlendTreeParameter = "Vert";

    [ShowInInspector, ReadOnly]
    Transform lookAtTarget;

    /// <summary>
    /// Set target for character to look at horizontally.
    /// </summary>
    /// <param name="target"></param>
    public void SetLookAtTarget(Transform target)
    {
        Debug.Log("[CharacterVisual]: Setting look at target: " + target);
        lookAtTarget = target;
    }
    void Update()
    {
        if (Moveable != null)
        {
            // todo update this for any speed
            if (Moveable.GetMoveSpeed() > 0.3f)
            {
                Animator.SetFloat(MovementBlendTreeParameter, 1);
            }
            else
            {
                Animator.SetFloat(MovementBlendTreeParameter, 0);
            }
        }

        HandleLookDir();
    }

    void HandleLookDir()
    {
        if (lookAtTarget == null) return;
        if (Moveable.GetMoveSpeed() > 0.1f) return;

        var dir = (lookAtTarget.position - transform.position).normalized;
        dir.y = 0;

        Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);

        VisualRoot.rotation = Quaternion.Lerp(VisualRoot.rotation, targetRot, Time.deltaTime * HorizontalRootLookAtSpeed);
    }



#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (lookAtTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, lookAtTarget.position);
        }
    }
#endif








}
