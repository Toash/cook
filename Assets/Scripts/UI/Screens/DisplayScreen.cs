using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents a screen in world space with a materical that has a render texture.
/// </summary>
[RequireComponent(typeof(MeshCollider))]
public class DisplayScreen : MonoBehaviour
{
    // screen raycast
    public LayerMask RaycastMask = ~0;
    public float MaxRaycastDistance = 50.0f;
    public UnityEvent<Vector2> OnCursorInput = new();


    // send normalized quad input to input relayer
    void Update()
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        Vector3 mousePos = Input.mousePosition;
#else
        Vector3 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
#endif


        // construct ray from mouse position
        // TODO: put this outside of this script.\
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
        // could also just use center point of screen (if mouse is not free)


        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit, MaxRaycastDistance, RaycastMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject != gameObject) return;

            // normalized position on the screen]
            OnCursorInput.Invoke(hit.textureCoord);

        }

    }

}