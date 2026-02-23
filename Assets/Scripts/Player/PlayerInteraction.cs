using Unity.Multiplayer.Center.Common.Analytics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float InteractRange = 3;

    public PlayerGrabber Grabber;
    public Transform CamRoot;
    public InputActionReference PrimaryInteract;
    public InputActionReference SecondaryInteract;
    public LayerMask InteractionMask;

    private IInteractable hoveredInteractable;

#if UNITY_EDITOR
    private Vector3 hitPoint;
#endif



    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(CamRoot.position, CamRoot.forward, out hit, InteractRange, InteractionMask))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                TryHighlightHoverInteractable(false);
                hoveredInteractable = interactable;
                TryHighlightHoverInteractable(true);

#if UNITY_EDITOR
                hitPoint = hit.point;
#endif
            }

        }
        else
        {
            TryHighlightHoverInteractable(false);
            hoveredInteractable = null;
        }

        if (PrimaryInteract.action.WasPressedThisFrame())
        {
            HandleInteraction(InteractType.Primary);
        }
        if (SecondaryInteract.action.WasPressedThisFrame())
        {
            HandleInteraction(InteractType.Secondary);
        }

    }

    void HandleInteraction(InteractType type)
    {
        InteractionContext context = new InteractionContext(type, this, Grabber);
        if (Grabber.isHolding)
        {
            Grabber.OnInteractAndHolding(context);
            return;
        }
        if (hoveredInteractable != null)
        {
            hoveredInteractable.Interact(context);
        }

    }


    void TryHighlightHoverInteractable(bool highlight)
    {
        if (hoveredInteractable == null) return;
        if (hoveredInteractable.GetTransform().TryGetComponent<Highlightable>(out Highlightable highlightable))
        {
            highlightable.SetHighlight(highlight);
        }

    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(CamRoot.position, CamRoot.forward * InteractRange);

        if (hoveredInteractable != null)
        {
            Handles.Label(hitPoint, "Interactable hit point");
            Gizmos.DrawWireSphere(hitPoint, .2f);

            Handles.Label(transform.position, "Current interactable: " + hoveredInteractable.GetTransform().name);
        }

    }
#endif
}