using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Contrains the camera to some position and rotation when interacted with.
/// </summary>
public class CameraConstrainer : InteractableBase
{
    [Tooltip("Where the camera will go when used.")]
    public Transform camTarget;
    public override void Interact(InteractionContext context)
    {
        // constrain the player camera
        context.Player.Controller.ConstrainCamera(camTarget.position, camTarget.rotation);
        context.Player.Controller.UnlockCursor();
    }

#if UNITY_EDITOR
    GUIStyle style = new GUIStyle();
    void OnDrawGizmosSelected()
    {
        if (camTarget == null) return;
        style.normal.textColor = Color.green;

        Gizmos.color = Color.green;
        Handles.Label(transform.position, "Computer");

        Gizmos.DrawSphere(camTarget.position, .3f);
        Gizmos.DrawRay(camTarget.position, ((camTarget.position + camTarget.forward) - camTarget.position) * 3);

        Handles.Label(camTarget.position, "Camera target");
    }
#endif
}