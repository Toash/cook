using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents a screen in world space with a materical that has a render texture.
/// </summary>
[RequireComponent(typeof(MeshCollider))]
public class DisplayScreen : MonoBehaviour
{

    public bool HasToBeConstrainedToUse = true;
    // screen raycast
    public LayerMask RaycastMask = ~0;
    public float MaxRaycastDistance = 5.0f;
    public UnityEvent<Vector2> OnCursorInput = new();


    private Player player;

    void Start()
    {
        //bruh 
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }


    // send normalized quad input to input relayer
    void Update()
    {
        if (HasToBeConstrainedToUse && !player.Controller.IsCameraContrained) return;
#if ENABLE_LEGACY_INPUT_MANAGER
        Vector3 mousePos = Input.mousePosition;
#else
        Vector3 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
#endif


        Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);


        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit, MaxRaycastDistance, RaycastMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject != gameObject) return;

            // normalized position on the screen]
            OnCursorInput.Invoke(hit.textureCoord);

        }

    }

}